/*
* @(#)AttachmentTest.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Test
{
    using System;
    using System.IO;
    using System.Text;
    using System.Xml;
    using System.Data;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;

    using Newtera.Common.Core;
    using Newtera.Data;
    using Newtera.Server.DB;
    using Newtera.Common.MetaData;
    using Newtera.Common.MetaData.Schema;
    using Newtera.Common.Attachment;

    /// <summary>
    /// Test program of CM.
    /// </summary>
    /// <version>  	1.0.0 26 Aug 2003 </version>
    /// <author>  		Yong Zhang </author>
    public class AttachmentTest
    {
        public const string ConnectionString = "";

        static List<Job> AllJobs = new List<Job> { new Job() { StartDate= "2017-02-01", EndDate="2017-03-12", Name="Move1" },
            new Job() { StartDate="2017-03-14", EndDate="2017-05-12", Name="Move2"}
        };

        static DateTime Today = new DateTime(2017, 7, 16);

        public static void Main()
        {
            List<Job> myJobs = GetJobs(AttachmentTest.AllJobs);

            Tree root = null;

            //InsertTree(ref root, 5, null);

            //InsertTree(ref root, 3, null);

            //InsertTree(ref root, 8, null);

            int[] values = new int[] { 12, 34, 23, 55, 67, 34 };

            Heap heap = new Heap();

            heap.MakeHeap(values);

            FixedSizeHashTable<string, string> hash = new FixedSizeHashTable<string, string>(20);

            hash.Add("1", "hello");
            hash.Add("2", "World");



            return;
        }


        static public List<Job> GetJobs(List<Job> allJobs)
        {
            List<Job> newJobList = new List<Job>();
            List<Job> myJobs = new List<Job>();

            allJobs.ForEach((job) =>
            {
                newJobList.Add(job);
            });

            Job currentJob;
            while (newJobList.Count > 0)
            {
                currentJob = GetEarlistCompleteJob(newJobList);

                myJobs.Add(currentJob);

                newJobList.Remove(currentJob);

                RemoveInterSectingJobs(newJobList, currentJob);

            }

            return null;
        }

        static Job GetEarlistCompleteJob(List<Job> jobList)
        {
            Job found = jobList[0];

            for (int i = 1; i < jobList.Count; i++)
            {
                if (DateTime.Parse(jobList[i].EndDate) < DateTime.Parse(found.EndDate))
                    found = jobList[i];
            }
            return found;
        }

        static void RemoveInterSectingJobs(List<Job> jobList, Job aJob)
        {
            for (int i = jobList.Count - 1; i >= 0; i--)
            {
                Job bJob = jobList[i];

                if (DateTime.Parse(aJob.StartDate) < DateTime.Parse(bJob.EndDate) &&
                    DateTime.Parse(bJob.StartDate) < DateTime.Parse(aJob.EndDate))
                    jobList.RemoveAt(i);
            }
        }

    }

    public class DayHalf
    {
        public const string AM = "AM";
        public const string PM = "PM";
    }

    // Job
    public class Job
    {
        public string StartDate;
        public string EndDate;
        public string Name;
    }

    public class Tree
    {
        public int Value;
        public Tree Parent;
        public Tree Left;
        public Tree Right;

        public Tree()
        {
            Parent = null;
            Left = null;
            Right = null;
        }
    }

    // movie schedule
    public class Movies
    {
        static void ScheduleMovies()
        {
            Dictionary<string, int[]> movies = new Dictionary<string, int[]>();
            movies.Add("AAA", new int[] { 4, 6, 7 });
            movies.Add("BBB", new int[] { 4, 7, 8 });
            movies.Add("DDD", new int[] { 6, 7, 10 });

            string[] movieNames = movies.Keys.ToArray();

            Dictionary<int, string> scheduler = new Dictionary<int, string>();

            if (MakeSchedule(movies, movieNames, 0, scheduler))
            {
                PrintSchedule(scheduler);
            }
            else
            {
                Console.WriteLine("No schedule");
            }
        }

        static bool MakeSchedule(Dictionary<string, int[]> movies, string[] movieNames, int i, Dictionary<int, string> schedule)
        {
            if (i >= movieNames.Length)
                return true;

            string movieName = movieNames[i];
            int[] times = movies[movieName];

            foreach (int time in times)
            {
                if (!schedule.ContainsKey(time))
                {
                    schedule[time] = movieName;
                    if (!MakeSchedule(movies, movieNames, i + 1, schedule))
                    {
                        schedule.Remove(time);
                    }
                    else
                        return true;
                }
            }

            return false;
        }

        static void PrintSchedule(Dictionary<int, string> schedule)
        {

        }
    }
}