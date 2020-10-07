using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JobManager : MonoBehaviour
{
    public Dictionary<Job, int> availableJobs;
    public static JobManager instance;

    private void Awake()
    {
        instance = this;
        availableJobs = new Dictionary<Job, int>();

        availableJobs.Add(Job.LUMBERJACK, 10);
    }

    public Role GetAvailableJob()
    {
        foreach (var job in availableJobs)
        {
            if (job.Value > 0)
            {
                return GetJobRole(job.Key);
            }   
        }

        return null;
    }

    private Role GetJobRole(Job job)
    {
        if (job == Job.LUMBERJACK)
        {
            return new Lumberjack();
        }

        return null;
    }

    public class NoAvailableJobsException : System.Exception
    {
    }
}
