using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Obsolete    //namespace는 소속을 나타내는 역할, Obsolete은 사용 안한다는 뜻 아닌가 왜 이런 이름을 지었을까
{

    public class PlayerMoverObsolete : MonoBehaviour
    {
        [SerializeField]
        private Camera cam;
        [SerializeField]
        private GameObject _player_object;
        private ManagerGroup _data_center;
        private Vector2 _player_logical_pos;
        private Dictionary<Vector2Int, MyTile> _tile_dictionary;
        private bool _is_moving;

        // Start is called before the first frame update
        void Start()
        {
            _data_center = ManagerGroup.singleton;
            _player_logical_pos = Vector2.zero;
            //_tile_dictionary = _data_center.tile_generator.tile_dictionary;
            _tile_dictionary = _data_center.tilemapGenerator.tileDictionary;
            _is_moving = false;
        }

        // Update is called once per frame
        void Update()
        {
            if (_is_moving == false)
            {
                if (Input.GetKey(KeyCode.DownArrow))
                {
                    SetMoveWithLogicalDelta(Vector2.down);
                }
                else if (Input.GetKey(KeyCode.LeftArrow))
                {
                    SetMoveWithLogicalDelta(Vector2.left);
                }
                else if (Input.GetKey(KeyCode.RightArrow))
                {
                    SetMoveWithLogicalDelta(Vector2.right);
                }
                else if (Input.GetKey(KeyCode.UpArrow))
                {
                    SetMoveWithLogicalDelta(Vector2.up);
                }
                if (Input.GetMouseButtonDown(1))
                {
                    RightClickMove(Input.mousePosition);
                }
            }
        }

        void SetMoveWithLogicalDelta(Vector2 _delta)
        {
            //Vector2Int _key = _player_logical_pos + _delta;
            Vector2Int _key = Vector2Int.zero;
            Debug.Log(_key);
            MyTile _dest_tile = _tile_dictionary[_key];
            Vector2 _dest = _dest_tile.actualPos;
            _player_logical_pos = _key;
            StartCoroutine(MovingCoroutine(_dest));
        }

        void SetMoveWithLogicalPos(Vector2Int _pos)
        {
            //Debug.Log(_pos);
            MyTile _dest_tile = _tile_dictionary[_pos];
            Vector2 _dest = _dest_tile.actualPos;
            _player_logical_pos = _pos;
            StartCoroutine(MovingCoroutine(_dest));
        }

        void RightClickMove(Vector2 _mouse_canvas_pos)
        {
            Vector2 _mouse_world_pos = cam.ScreenToWorldPoint(_mouse_canvas_pos);
            SetMoveWithLogicalPos(MyTile.ActualToLogicalPosition(_mouse_world_pos));
        }


        IEnumerator MovingCoroutine(Vector2 _destination)
        {
            _is_moving = true;
            float _timer = 0;
            Vector2 _origin = _player_object.transform.position;
            while (_timer < 1)
            {
                _player_object.transform.position = Vector2.Lerp(_origin, _destination, _timer);
                _timer += Time.deltaTime * 2;
                yield return null;
            }
            _player_object.transform.position = _destination;
            _is_moving = false;
        }

    }

}