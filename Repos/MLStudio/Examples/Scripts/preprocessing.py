# Data preparation script
# This script produces 2 files: training and validation files 12 steps prediction horizon. 
import os, sys
import pandas as pd
import numpy as np
from pathlib import Path
from pandas import Series, DataFrame, Panel
import statsmodels.api as sm

DATA_FILE = "cif-dataset.txt"
TRAIN_FILE = "data\Train_Data.txt"
TEST_FILE = "data\Test_Data.txt"
EVAL_FILE = "data\Eval_Data.txt"

if len(sys.argv) > 4 :
    DATA_FILE = sys.argv[1]
    TRAIN_FILE = sys.argv[2]
    TEST_FILE = sys.argv[3]
    EVAL_FILE = sys.argv[4]

print(DATA_FILE)
print(TRAIN_FILE)
print(TEST_FILE)
print(EVAL_FILE)

os.getcwdb()
OUTPUT_DIR="data"
if not os.path.isdir(OUTPUT_DIR) :
    os.mkdir(OUTPUT_DIR, )
# read file
cif_df = pd.read_table(DATA_FILE, sep=";", header=None)
# add column names
names = ["Series", "maxPredHorizon", "Freq"]
cols = cif_df.shape[1] - 2
for i in range(1, cols) :
    names.append("x_" + str(i))

cif_df.columns = names;

#print(cif_df)

inputSize = 15
outputSize = 12

for fileType in ('TRAIN', 'TEST', 'EVAL'):
    if fileType == 'TEST' :
        OUTPUT_PA12 = TEST_FILE
    elif fileType == 'TRAIN' :
        OUTPUT_PA12 = TRAIN_FILE
    else:
        OUTPUT_PA12 = EVAL_FILE

    OUTPUT_FILE = Path(OUTPUT_PA12)
    if OUTPUT_FILE.is_file():
        os.remove(OUTPUT_PA12)

    thefile = open(OUTPUT_PA12, 'w')

    save12_df=None
    rows = len(cif_df.index)  # rows of loaded file
    for idr in range(rows):
        oneLine_df = cif_df.iloc[[idr]]
        series = oneLine_df.iloc[0]['Series']
        maxForecastHorizon = oneLine_df.iloc[0]['maxPredHorizon']
        if maxForecastHorizon != 12: # ignore the 6 month forecast
            continue
        y = oneLine_df.iloc[0][3:]
        y = y.dropna()
        y = pd.to_numeric(y)
        ylog = y.apply(np.log)
        n = ylog.size
        
        if fileType == 'TRAIN' :
            n=n-maxForecastHorizon # leave the last maxForecastHorizon numbers for validation
            ylog=ylog[0:n]

        nn_vect = ylog.values
        nnLevels = ylog.values
        
        #ylist = ylog.values
        #decomp = sm.tsa.seasonal_decompose(ylist, freq=12)
        #n = n -12
        #nnLevels = decomp.trend[6:n + 6]
        #print(nnLevels)
        #resid = decomp.resid[6:n + 6]
        #print(resid)
        #nn_vect = np.array(nnLevels) + np.array(resid)
        #print(nn_vect)
        
        print(series)
        
        if fileType == 'EVAL' :
            end = n + 1 # use all data for evaluation
        else:
            end = n - maxForecastHorizon + 1
        
        for inn in range(inputSize, end) :
            level = nnLevels[inn - 1] #last "trend" point in the input window is the "level" (the value used for the normalization)
            sav_df = [] #creates an empty list
            sav_df.append(str(idr + 1) + '|features')

            for ii in range(inputSize) :
                val = nn_vect[inn-inputSize+ii] - level
                sav_df.append(' ' + str(val))
            
            if fileType != 'EVAL' :  # eval file doesn't need labels
                sav_df.append(' |labels')
                for ii in range(maxForecastHorizon) :
                    val = nn_vect[inn+ii] - level
                    sav_df.append(' ' + str(val)) 

            sav_df.append(' |#')
            sav_df.append(' ' + str(level))
            
            for item in sav_df :
                thefile.write("%s" % item)


            thefile.write("\n");
thefile.flush()
thefile.close()
