using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PeopleController : MonoBehaviour
{
    private float speed = 1.5f;
    private int birthDate;
    private int deathAge;
    private int healthPercent;
    private long actionEndTick;
    private Vector3 patrolPoint;
    private float patrolPathLength;
    private Action currentAction;
    private int gold;
    private Role workingRole;
    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();

        birthDate = PlayerPrefs.GetInt(GlobalConstants.GAME_MILLIS);
        SetRandomDeathAge();
        bool unemployed = GetAJob();
        if (unemployed) {
            SetRandomAction();
        }

        Debug.Log("Spawned a man with death age " + deathAge + " years");
    }

    private bool GetAJob()
    {
        var job = JobManager.instance.GetAvailableJob();
        workingRole = job;
        if (job == null)
        {
            Debug.Log("No job");
            return true;
        }

        Debug.Log("Got a job " + workingRole);
        workingRole.Init(this);
        
        return false;
    }

    public void SetRandomAction()
    {
        // Indices 0 to 2 is random
        currentAction = (Action) Random.Range(0, 2);
        if (currentAction == Action.IDLE)
        {
            ConfigureIdleAction();
        }
        else if (currentAction == Action.PATROLING)
        {
            patrolPoint = GetRandomPoint(GlobalConstants.PEOPLE_PATROL_RADIUS);
            var startPoint2d = new Vector3(transform.position.x, 0, transform.position.z);
            var patrolPoint2d = new Vector3(patrolPoint.x, 0, patrolPoint.z);

            patrolPathLength = Vector3.Distance(startPoint2d, patrolPoint2d);
        }

        Debug.Log("Set action " + currentAction + " with patrolPoint " + patrolPoint + " and idleTime " + actionEndTick);
    }

    private void ConfigureIdleAction()
    {
        var ticksLength = Random.Range(500, GlobalConstants.MAX_PEOPLE_IDLE_MILLIS);
        actionEndTick = PlayerPrefs.GetInt(GlobalConstants.GAME_MILLIS) + ticksLength;
    }

    private Vector3 GetRandomPoint(int patrolRadius)
    {
        var currentPoint = transform.position;
        var xDiff = Random.Range(-patrolRadius, patrolRadius);
        var zDiff = Random.Range(-patrolRadius, patrolRadius);

        return new Vector3(currentPoint.x + xDiff, currentPoint.y, currentPoint.z + zDiff);
    }

    private void SetRandomDeathAge()
    {
        deathAge = Random.Range(0, 100);
    }

    // Update is called once per frame
    void Update()
    {
        if (GetAge() >= deathAge)
        {
            PlayDeath();
        }

        PayTaxes();

        DoAction();
        
        if (!ActionAcomplished())
        {
            return;
        }
        
        if (HasJob()) 
        {
            workingRole.ActionAccomplished(this);
        }
        else
        {
            SetRandomAction();
        }
    }

    private bool HasJob()
    {
        return workingRole != null;
    }

    private int GetAge()
    {
        return (PlayerPrefs.GetInt(GlobalConstants.GAME_MILLIS) - birthDate) / GlobalConstants.MILLIS_PER_AGE;
    }

    private void PlayDeath()
    {
        Utility.AddToIntProperty(GlobalConstants.PEOPLE_COUNT, -1);
        this.gameObject.SetActive(false);
    }

    private void PayTaxes()
    {
        if (gold == 0) return;
        var taxesToPay = GlobalConstants.TAXES_PER_SECOND * Time.deltaTime + 1;
        //Utility.AddToIntProperty(GlobalConstants.GOLD, (int) taxesToPay);
        gold -= (int) taxesToPay;
    }

    public void Idle(float seconds)
    {
        actionEndTick = (long) (PlayerPrefs.GetInt(GlobalConstants.GAME_MILLIS) + seconds * 1000);
        currentAction = Action.IDLE;
    }

    public void LoopSound(string clipName)
    {
        var clip = SoundManager.instance.FindClip(clipName);

        audioSource.loop = true;
        audioSource.PlayOneShot(clip);
    }

    public void PlayOneShot(string clipName)
    {
        var clip = SoundManager.instance.FindClip(clipName);

        audioSource.PlayOneShot(clip);
    }

    public void StopSound()
    {
        audioSource.Stop();
    }

    private void DoAction()
    {
        if (currentAction == Action.PATROLING || currentAction == Action.GO_TO_JOB)
        {
            var pathDiff = Time.deltaTime * speed;
            var newPosition = Vector3.Lerp(transform.position, patrolPoint, pathDiff / patrolPathLength);
            patrolPathLength -= pathDiff;

            newPosition.y = 1000; // in order to raycast to the ground
            transform.position = Utility.GetGroundedPoint(newPosition);
        }
    }
    
    private bool ActionAcomplished()
    {
        if (currentAction == Action.IDLE)
        {
            if (PlayerPrefs.GetInt(GlobalConstants.GAME_MILLIS) > actionEndTick)
            {
                return true;
            }
        }
        else if (currentAction == Action.PATROLING || currentAction == Action.GO_TO_JOB)
        {
            return patrolPathLength <= GlobalConstants.DISTANCE_DIFF_DELTA;
        }

        return false;
    }

    public void GoTo(Vector3 target)
    {
        currentAction = Action.GO_TO_JOB;
        patrolPoint = target;
        patrolPathLength = Vector3.Distance(patrolPoint, transform.position);

        Debug.Log("Go to " + target);
    }

    enum Action
    {
        IDLE,
        PATROLING,
        LOOKING_4_JOB,
        WORKING,
        GO_TO_JOB
    }
}
