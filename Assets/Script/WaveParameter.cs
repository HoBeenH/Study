using System;
using System.Collections.Generic;
using Script.Custom.Extensions;

namespace Script.Parameter.Struct
{
    public record WaveParameter
    {
        private readonly Dictionary<int, int[]> WaveEnemyDic = new Dictionary<int, int[]>();
        
        public int TotalCount { get; private set; }
        public int TotalWave => WaveEnemyDic.Count + 1;
        
        public void Add(int wave, int[] enemyArr)
        {
            if (WaveEnemyDic.ContainsKey(wave))
                throw new Exception($"Already Contains Wave {wave.ToString()}");
            
            WaveEnemyDic.Add(wave, enemyArr);
            TotalCount += enemyArr.Length;
        }

        public int[] GetCurrentWaveEnemy(int wave)
        {
            WaveEnemyDic.TryGetValue(wave, out var _result);
            return _result;
        }

        public List<int> GetAllActorList()
        {
            var _result = new List<int>();
            foreach (var pair in WaveEnemyDic)
            {
                var _waveEnemyList = pair.Value;
                if (!_waveEnemyList.IsNullOrEmptyCollection())
                    _result.AddRange(_waveEnemyList);
            }

            return _result;
        }
    }
}