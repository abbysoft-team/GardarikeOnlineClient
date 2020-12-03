using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GlobalConstants
{
    // Property constants 
    public const string GAME_MILLIS = "GameMillis";
    public const string PEOPLE_COUNT = "Population";
    public const string POPULATION_MAX_COUNT = "MaxPopulation";
    public const string GOLD = "Gold";

    public const int MILLIS_PER_AGE = 1000 * 2 * 60 * 60;
    public const int MAX_PEOPLE_IDLE_MILLIS = 1000 * 10;
    public const int PEOPLE_PATROL_RADIUS = 25;
    public const float DISTANCE_DIFF_DELTA = 0.5f;

    public const int TAXES_PER_SECOND = 10;

    public const int TREE_MIN_COUNT = 20;
    public const int TREE_MAX_COUNT = 100;
    public const int FOOD_MIN_COUNT = 10;
    public const int FOOD_MAX_COUNT = 50;
    public const int MIN_WOOD_PER_TREE = 10;
    public const int MAX_WOOD_PER_TREE = 300;
    public const float WOOD_CUT_SPEED = 5;

    public const float TREE_GROW_DELTA = 0.05f;

    public const float CHUNK_SIZE = 10000f;

    public const float WATER_LEVEL = -18.7f;

    public const int POPULATION_PER_HOME = 10;

    public const string SAVE_PREFIX = "Saving_";
    public const string REFERENCE_TAG = "Reference_";
    public const string BUILDING_TAG = "BUILDING";
    public const string COUNTRY_NAME_PROPERTY = "country_name";
    public const string TUTORIAL_COMPLETE_PROPERTY = "tutorial_complete";

}
