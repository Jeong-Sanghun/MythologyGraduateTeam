using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Obsolete
{

    public class PlayerMoveObsolete1 : MonoBehaviour
    {
        enum Direction
        {
            Down, Left, Right, Up
        }
        [SerializeField]
        private Camera cam;
        [SerializeField]
        private GameObject _player_object;
        private ManagerGroup _data_center;
        private Vector2 _player_logical_pos;
        private Dictionary<Vector2Int, MyTile> _tile_dictionary;
        private Vector2 _move_direction_normalized;
        private Vector2 _move_direction;
        private Vector2 _origin_move_direction_normalized;
        private bool _now_colliding;

        private bool[] _key_move_array;
        private MyTile _now_stand_tile;
        private float[] _speed_array;
        private int _speed_index;
        private const float _player_collider_radius = 0.2f;
        // Start is called before the first frame update
        void Start()
        {
            _data_center = ManagerGroup.singleton;
            _player_logical_pos = Vector2.zero;
            //_tile_dictionary = _data_center.tile_generator.tile_dictionary;
            _tile_dictionary = _data_center.tilemapGenerator.tileDictionary;
            _speed_array = new float[] { 0.5f, 1f, 2f, 1.5f, 1f, 0.5f };
            _speed_index = 0;
            _key_move_array = new bool[4];
            _now_colliding = false;
            for (int i = 0; i < _key_move_array.Length; i++)
            {
                _key_move_array[i] = false;
            }

        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                SetKeyboardMove(Direction.Down, true);
            }
            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                SetKeyboardMove(Direction.Left, true);
            }
            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                SetKeyboardMove(Direction.Right, true);
            }
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                SetKeyboardMove(Direction.Up, true);
            }

            if (Input.GetKeyUp(KeyCode.DownArrow))
            {
                SetKeyboardMove(Direction.Down, false);
            }
            if (Input.GetKeyUp(KeyCode.LeftArrow))
            {
                SetKeyboardMove(Direction.Left, false);
            }
            if (Input.GetKeyUp(KeyCode.RightArrow))
            {
                SetKeyboardMove(Direction.Right, false);
            }
            if (Input.GetKeyUp(KeyCode.UpArrow))
            {
                SetKeyboardMove(Direction.Up, false);
            }

            //if (Input.GetMouseButtonDown(1))
            //{
            //    RightClickMove(Input.mousePosition);
            //}
            //if (Input.GetMouseButtonUp(1))
            //{
            //    _move_direction_normalized = Vector2.zero;
            //}
            Move();

        }

        private MyTile UpdateNowStandTile(Vector2 _pos)
        {
            return _tile_dictionary[MyTile.ActualToLogicalPosition(_pos)];
        }

        private List<MyTile> UpdateNowStandCollidingTile(Vector2 _pos)
        {
            List<MyTile> _tile_list = new List<MyTile>();
            bool _toggle = true;
            for (float i = -_player_collider_radius; i <= _player_collider_radius; i += _player_collider_radius / 4f)
            {
                for (int j = 0; j < 2; j++)
                {
                    float _x = i;
                    float _y;
                    if (_player_collider_radius * _player_collider_radius - i * i <= 0)
                    {
                        _y = 0;
                    }
                    else
                    {
                        if (_toggle)
                        {
                            _y = Mathf.Sqrt(_player_collider_radius * _player_collider_radius - i * i);
                        }
                        else
                        {
                            _y = -Mathf.Sqrt(_player_collider_radius * _player_collider_radius - i * i);
                        }
                    }

                    _toggle = !_toggle;
                    Vector2 _actual_pos = new Vector2(_x, _y);
                    _actual_pos += _pos;
                    _gizmo = _actual_pos;
                    MyTile _tile = _tile_dictionary[MyTile.ActualToLogicalPosition(_actual_pos)];
                    if (!_tile_list.Contains(_tile))
                    {
                        _tile_list.Add(_tile);
                    }
                }

            }
            return _tile_list;

        }


        void SetKeyboardMove(Direction _direction, bool _pressing)
        {
            _now_colliding = false;
            _key_move_array[(int)_direction] = _pressing;
            _move_direction = Vector2.zero;
            for (int i = 0; i < _key_move_array.Length; i++)
            {
                if (_key_move_array[i] == true)
                {
                    switch ((Direction)i)
                    {
                        case Direction.Down:
                            _move_direction += Vector2.down;
                            break;
                        case Direction.Left:
                            _move_direction += Vector2.left;
                            break;
                        case Direction.Right:
                            _move_direction += Vector2.right;
                            break;
                        case Direction.Up:
                            _move_direction += Vector2.up;
                            break;

                    }

                }
            }
            _origin_move_direction_normalized = _move_direction;
            _origin_move_direction_normalized.Normalize();
        }

        //void RightClickMove(Vector2 _mouse_canvas_pos)
        //{
        //    Vector2 _mouse_world_pos = cam.ScreenToWorldPoint(_mouse_canvas_pos);
        //    _move_direction_normalized = _mouse_world_pos-(Vector2)_player_object.transform.position;
        //    _move_direction_normalized.Normalize();
        //}
        void RightClickMove(Vector2 _mouse_canvas_pos)
        {
            Vector2 _mouse_world_pos = cam.ScreenToWorldPoint(_mouse_canvas_pos);
            SetMoveWithLogicalPos(MyTile.ActualToLogicalPosition(_mouse_world_pos));
        }
        void SetMoveWithLogicalPos(Vector2Int _pos)
        {
            //Debug.Log(_pos);
            MyTile _dest_tile = _tile_dictionary[_pos];
            Vector2 _dest = _dest_tile.actualPos;
            _player_logical_pos = _pos;
            StartCoroutine(MovingCoroutine(_dest));
        }


        IEnumerator MovingCoroutine(Vector2 _destination)
        {
            float _timer = 0;
            Vector2 _origin = _player_object.transform.position;
            while (_timer < 1)
            {
                _player_object.transform.position = Vector2.Lerp(_origin, _destination, _timer);
                _timer += Time.deltaTime * 2;
                yield return null;
            }
            _player_object.transform.position = _destination;
        }

        Vector2 _gizmo;

        void Move()
        {
            float _speed = 1;//_speed_array[_speed_index++ % _speed_array.Length];
            Vector2 _processed_normal = _origin_move_direction_normalized;
            Vector2 _next_pos = (Vector2)_player_object.transform.position + _origin_move_direction_normalized * Time.deltaTime * _speed;
            List<MyTile> _tile_List = UpdateNowStandCollidingTile((Vector2)_player_object.transform.position);
            List<Vector2> _minus_list = new List<Vector2>();
            MyTile _tile = null;
            //if (_tile_List.Count == 0)
            //{
            //    if (_now_colliding == true)
            //    {
            //        Debug.Log("원래대로 돌아옴");
            //        _move_direction_normalized = _origin_move_direction_normalized;
            //    }
            //    _now_colliding = false;

            //}
            //else
            //{
            //    _now_colliding = true;
            //}

            int times = 0;
            for (int i = 0; i < _tile_List.Count; i++)
            {
                _tile = _tile_List[i];

            }
            Vector2 _minus_added = new Vector2();
            for (int i = 0; i < _minus_list.Count; i++)
            {

                _minus_added += _minus_list[i];

            }
            if (_minus_list.Count > 0)
            {
                _minus_added.Normalize();
                _processed_normal.Normalize();
                float _dotted = Vector2.Dot(_processed_normal, _minus_added);

                Debug.Log("애디드  " + _minus_added);
                if (_dotted > 0)
                {
                    _processed_normal -= _dotted * _minus_added;
                }

            }

            _next_pos = (Vector2)_player_object.transform.position + _processed_normal * Time.deltaTime * _speed;

            _player_object.transform.position = _next_pos;
            _now_stand_tile = UpdateNowStandTile(_next_pos);
            //}
            //_player_object.transform.position = _next_pos;
            //_now_stand_tile = _tile;
        }


        private void OnDrawGizmos()
        {
            Gizmos.DrawCube(_gizmo, new Vector3(0.1f, 0.1f));
            Gizmos.DrawCube((Vector2)_player_object.transform.position, new Vector3(0.1f, 0.1f));
        }
    }

}