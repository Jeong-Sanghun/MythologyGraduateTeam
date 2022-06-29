using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Obsolete
{

    public class TileGenerator : MonoBehaviour
    {
        ManagerGroup data_center;
        [SerializeField]
        private GameObject _tile_collider_prefab;
        [SerializeField]
        private Transform _tile_parent;
        public Dictionary<Vector2, MyTile> tile_dictionary;
        void GenerateTile()
        {
            tile_dictionary = new Dictionary<Vector2, MyTile>();
            for (int i = -40; i < 40; i++)
            {
                for (int j = -40; j < 40; j++)
                {
                    MyTile _tile = new MyTile();
                    Vector2 _pos = new Vector2(i, j);
                    //_tile.SetValue(_pos, _tile_collider_prefab, _tile_parent);
                    tile_dictionary.Add(_pos, _tile);
                }

            }
        }
        // Start is called before the first frame update
        void Start()
        {
            data_center = ManagerGroup.singleton;
            //data_center.tile_generator = this;
            GenerateTile();
        }

        // Update is called once per frame
        void Update()
        {

        }
    }

}