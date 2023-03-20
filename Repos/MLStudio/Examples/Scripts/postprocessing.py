# Data postparation script
# This script produces 1 file for restored forecasting data. 
import os, sys
import pandas as pd
import numpy as np
from pathlib import Path
from pandas import Series, DataFrame, Panel
import statsmodels.api as sm

def writeForecast(thefile, currentTSNum, lastForecast, lastLevel): 
    print(currentTSNum)
    lastForecast = [x + lastLevel for x in lastForecast]
    lastForecast = pd.to_numeric(lastForecast)
    lastForecast = np.exp(lastForecast)
    restored_forecast = [] #creates an empty list
    restored_forecast.append('ts' + currentTSNum + '|')
    for ii in range(lastForecast.size) :
        restored_forecast.append(' ' + str(lastForecast[ii]))
    for item in restored_forecast :
        thefile.write("%s" % item)
    thefile.write("\n");

EVAL_FILE = "Eval_Data.txt"
INPUT_FILE = "output.txt.z"
OUTPUT_FILE = "Output_Data.txt"

if len(sys.argv) > 3 :
    EVAL_FILE = sys.argv[1]
    INPUT_FILE = sys.argv[2]
    OUTPUT_FILE = sys.argv[3]
#print(EVAL_FILE)
#print(INPUT_FILE)
#print(OUTPUT_FILE)

cwd = os.getcwdb()
# read file
input_df = pd.read_table(INPUT_FILE, sep=" ", header=None)
eval_df = pd.read_table(EVAL_FILE, sep=" ", header=None)

output_path = Path(OUTPUT_FILE)
if output_path.is_file():
    os.remove(OUTPUT_FILE)

thefile = open(OUTPUT_FILE, 'w')

rows = len(input_df.index)  # rows of loaded input file and test file, both of them should have same len
currentTSNum = None
lastForecast = None
lastLevel = None
for idr in range(rows):
    oneLine_input = input_df.iloc[[idr]]
    oneLine_eval = eval_df.iloc[[idr]]
    forecast = oneLine_input.iloc[0][0:]
    evaldata = oneLine_eval.iloc[0][0:]
    levelIndex = evaldata[evaldata == '|#'].index[0] + 1
    level = evaldata[levelIndex]

    ts = evaldata[0]
    index = ts.index('|')
    tsNum = ts[0:index] # get time series number
    if currentTSNum == None :
        currentTSNum = tsNum
    
    # only write out the last forecast record of a time series
    if tsNum != currentTSNum :
        writeForecast(thefile, currentTSNum, lastForecast, lastLevel)
        currentTSNum = tsNum
    else:
        lastForecast = forecast
        lastLevel = level

# last record
writeForecast(thefile, currentTSNum, lastForecast, lastLevel)
thefile.flush()
thefile.close()

