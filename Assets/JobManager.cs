using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Gardarike;

public class JobManager : MonoBehaviour
{
    public Dictionary<Job, int> availableJobs;
    public static JobManager instance;

    private void Awake()
    {
        instance = this;
        availableJobs = new Dictionary<Job, int>();
        //availableJobs[Job.LUMBERJACK] = 
        
        EventBus.instance.onCharacterSelected += LoadJobInfo;
        //EventBus.instance.onJobMarketInfoArrived += UpdateJobsInfo;
    }

    private void LoadJobInfo()
    {
        EventBus.instance.RequestJobMarketInfo();
    }

    private void UpdateJobsInfo(GetWorkDistributionResponse workDistribution)
    {
        //availableJobs
    }

    public Role GetAvailableJob()
    {
        foreach (var job in availableJobs)
        {
            if (job.Value > 0)
            {
                var role = GetJobRole(job.Key);
                if (role != null)
                {
                    availableJobs[job.Key] = availableJobs[job.Key] - 1;
                }

                return role;
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
