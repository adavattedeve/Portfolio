using UnityEngine;
using System.Collections;

public class BloodSplatCreator {

    public enum SplatSize{SMALL, MEDIUM, LARGE };

    private static bool inited = false;
    //private static BloodSplatParams small;
    //private static BloodSplatParams medium;
    //private static BloodSplatParams large;
    //private static ObjectToBePooled bloodSplatPoolData;

    private static ObjectToBePooled smallBloodSplatPoolData;
    private static ObjectToBePooled mediumBloodSplatPoolData;
    private static ObjectToBePooled largeBloodSplatPoolData;

    public static void CreateBloodSplats(Vector3 position, SplatSize size)
    {
        if (!inited)
            Init();
        
        switch(size) {
            case SplatSize.SMALL:
                //CreateSplats(small, position);
                ObjectPool.instance.GetObjectFromPool(smallBloodSplatPoolData, position, Quaternion.identity);
                break;

            case SplatSize.MEDIUM:
                //CreateSplats(medium, position);
                ObjectPool.instance.GetObjectFromPool(mediumBloodSplatPoolData, position, Quaternion.identity);
                break;

            case SplatSize.LARGE:
                //CreateSplats(large, position);
                ObjectPool.instance.GetObjectFromPool(largeBloodSplatPoolData, position, Quaternion.identity);
                break;
        }
        
        
    }

    private static void Init() {
        smallBloodSplatPoolData = new ObjectToBePooled();
        smallBloodSplatPoolData.path = Constants.EFFECTPATH + "BloodSplatSmall";
        ObjectPool.instance.AddNewObjectToBePooled(smallBloodSplatPoolData);

        mediumBloodSplatPoolData = new ObjectToBePooled();
        mediumBloodSplatPoolData.path = Constants.EFFECTPATH + "BloodSplatMedium";
        ObjectPool.instance.AddNewObjectToBePooled(mediumBloodSplatPoolData);

        largeBloodSplatPoolData = new ObjectToBePooled();
        largeBloodSplatPoolData.path = Constants.EFFECTPATH + "BloodSplatLarge";
        ObjectPool.instance.AddNewObjectToBePooled(largeBloodSplatPoolData);
        /*
        bloodSplatPoolData = new ObjectToBePooled();
        bloodSplatPoolData.path = Constants.EFFECTPATH + "BloodSplat";
        ObjectPool.instance.AddNewObjectToBePooled(bloodSplatPoolData);
        small = new BloodSplatParams(1, 2, 0.2f, 0.8f);
        medium = new BloodSplatParams(2, 4, 0.5f, 1.2f);
        large = new BloodSplatParams(3, 6, 0.7f, 1.5f);
        */
        inited = true;
    }
    /*
    private static void CreateSplats(BloodSplatParams parameters, Vector3 position)
    {
        int count = Random.Range(parameters.splatCountRangeMin, parameters.splatCountRangeMax + 1);

        for (int i = 0; i < count; ++i)
        {
            GameObject splat = ObjectPool.instance.GetObjectFromPool(bloodSplatPoolData, position, Quaternion.identity) as GameObject;
            splat.transform.localScale = new Vector3(Random.Range(parameters.scaleMin, parameters.scaleMax), 1, Random.Range(parameters.scaleMin, parameters.scaleMax));
            splat.transform.Rotate(Vector3.up, Random.Range(0, 360));
        }

    }

    

    private class BloodSplatParams
    {
        public int splatCountRangeMin = 2;
        public int splatCountRangeMax = 5;

        [Range(0.1f, 5)]
        public float scaleMin = 0.5f;
        [Range(0.1f, 5)]
        public float scaleMax = 2f;

        public BloodSplatParams (int _splatCountRangeMin, int _splatCountRangeMax, float _scaleMin, float _scaleMax) {
            splatCountRangeMin = _splatCountRangeMin;
            splatCountRangeMax = _splatCountRangeMax;
            scaleMin = _scaleMin;
            scaleMax = _scaleMax;
        }
    }
    */
}
