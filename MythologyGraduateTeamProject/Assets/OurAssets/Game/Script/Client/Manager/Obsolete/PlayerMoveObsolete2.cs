using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Obsolete
{

    public class PlayerMoveObsolete2 : MonoBehaviour
    {
        enum Direction
        {
            Down, Left, Right, Up
        }
        [SerializeField]
        private Camera cam;
        [SerializeField]
        private GameObject _player_object;
        private Rigidbody2D _player_rigid;
        private ManagerGroup _data_center;
        private Vector2 _player_logical_pos;
        private Dictionary<Vector2Int, MyTile> _tile_dictionary;
        private Vector2 _move_direction_normalized;
        private Vector2 _move_direction;
        private Vector2 _origin_move_direction_normalized;


        private bool[] _key_move_array;
        private MyTile _now_stand_tile;
        private float[] _speed_array;
        private int _speed_index;
        private const float _player_collider_radius = 0.2f;
        private float _speed_timer = 0;
        private float _mouse_right_click_maintain_time = 0;
        private Vector2 _mouse_click_move_destination_pos;
        private MyTile _mouse_click_move_destination_tile;
        private bool _keyboard_moving;
        private bool _mouse_moving;
        private bool _path_finding;
        private bool _path_finding_toggle;
        private float _now_path_length;
        private List<Vector2> _gizmo_list;

        // Start is called before the first frame update
        void Start()
        {
            _gizmo_list = new List<Vector2>();
            _data_center = ManagerGroup.singleton;
            _player_logical_pos = Vector2.zero;
            _player_rigid = _player_object.GetComponent<Rigidbody2D>();
            //_tile_dictionary = _data_center.tile_generator.tile_dictionary;
            _tile_dictionary = _data_center.tilemapGenerator.tileDictionary;
            _speed_array = new float[] { 0.5f, 1f, 2f, 1.5f, 1f, 0.5f };
            _speed_index = 0;
            _keyboard_moving = false;
            _mouse_moving = false;
            _path_finding = false;
            _path_finding_toggle = true;
            _key_move_array = new bool[4];
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

            if (Input.GetMouseButtonDown(1))
            {
                OnMouseRightClickDown(Input.mousePosition);
            }
            //if (Input.GetMouseButton(1))
            //{
            //    OnMouseRightClickMaintain(Input.mousePosition);
            //}
            //if (Input.GetMouseButtonUp(1))
            //{
            //    OnMouseRightClickUp(Input.mousePosition);
            //}
            Move();

        }

        private List<MyTile> GetCollidingTilesInRadius(Vector2 _pos)
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
                    MyTile _tile = _tile_dictionary[MyTile.ActualToLogicalPosition(_actual_pos)];
                    if (!_tile_list.Contains(_tile))
                    {
                        _tile_list.Add(_tile);
                    }
                }

            }
            return _tile_list;

        }


        private MyTile GetTileWithActualPos(Vector2 _pos)
        {
            return _tile_dictionary[MyTile.ActualToLogicalPosition(_pos)];
        }

        void SetKeyboardMove(Direction _direction, bool _pressing)
        {
            _keyboard_moving = false;
            _key_move_array[(int)_direction] = _pressing;
            _move_direction = Vector2.zero;
            for (int i = 0; i < _key_move_array.Length; i++)
            {
                if (_key_move_array[i] == true)
                {
                    _keyboard_moving = true;
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
            if (_mouse_moving == true)
            {
                _keyboard_moving = false;
                return;
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
        private void OnMouseRightClickDown(Vector2 _mouse_canvas_pos)
        {
            if (_keyboard_moving == true)
            {
                return;
            }
            _mouse_moving = true;
            Vector2 _mouse_world_pos = cam.ScreenToWorldPoint(_mouse_canvas_pos);
            _mouse_right_click_maintain_time = 0;
            Vector2 _end_pos = GetTileWithActualPos(_mouse_world_pos).actualPos;
            _now_path_length = 0;
            List<Vector2> _path_list = GetShortestPath(_player_object.transform.position, _end_pos);
            _path_finding_toggle = !_path_finding_toggle;
            if (_path_list != null)
            {
                _gizmo_list = _path_list;
                StartCoroutine(MoveToPath(_path_list));
            }
            else
            {
                Vector2 _direction = _end_pos - (Vector2)_player_object.transform.position;
                _direction.Normalize();
                _origin_move_direction_normalized = _direction;
            }
        }

        private void OnMouseRightClickMaintain(Vector2 _mouse_canvas_pos)
        {
            if (_keyboard_moving == true)
            {
                return;
            }
            Vector2 _mouse_world_pos = cam.ScreenToWorldPoint(_mouse_canvas_pos);
            _mouse_right_click_maintain_time += Time.deltaTime;
            _origin_move_direction_normalized = _mouse_world_pos - (Vector2)_player_object.transform.position;
            _origin_move_direction_normalized.Normalize();
        }

        private void OnMouseRightClickUp(Vector2 _mouse_canvas_pos)
        {
            if (_keyboard_moving == true)
            {
                return;
            }
            _mouse_moving = false;
            if (_mouse_right_click_maintain_time > 0.3f)
            {
                _origin_move_direction_normalized = Vector2.zero;

                return;
            }
            Vector2 _mouse_world_pos = cam.ScreenToWorldPoint(_mouse_canvas_pos);
            _mouse_click_move_destination_pos = _mouse_world_pos;

        }

        void Move()
        {
            if (_path_finding == true)
            {
                return;
            }

            _speed_timer += Time.deltaTime;
            if (_speed_timer > 0.2)
            {
                _speed_timer = 0;
                _speed_index++;
            }
            _player_rigid.velocity = _origin_move_direction_normalized * _speed_array[_speed_index % _speed_array.Length];
            _now_stand_tile = GetTileWithActualPos(_player_object.transform.position);
            List<MyTile> _tile_List = GetCollidingTilesInRadius((Vector2)_player_object.transform.position);
            for (int i = 0; i < _tile_List.Count; i++)
            {
                _data_center.tilemapGenerator.SetColliderObject(_tile_List[i]);
            }
        }

        List<Vector2> GetShortestPath(Vector2 _start, Vector2 _end)
        {
            List<Vector2> _vec_list = new List<Vector2>();
            _vec_list.Add(_start);
            Vector2 _first_collided_pos = GetFirstCollidePosition(_start, _end);

            if (_first_collided_pos == Vector2.zero)
            {
                _now_path_length += (_end - _start).magnitude;
                if (_now_path_length > 10)
                {
                    return null;
                }
                _vec_list.Add(_end);
                return _vec_list;
            }
            else
            {
                Vector2 _line_vec = _start - _end;
                Vector2 _plus_vec = new Vector2(1, _line_vec.x / _line_vec.y);
                Vector2 _minus_vec = -_plus_vec;
                _plus_vec.Normalize();
                _minus_vec.Normalize();
                _plus_vec *= 5;
                _minus_vec *= 5;
                Vector2 _end_collide_pos1 = GetEndCollidePosition(_first_collided_pos, _first_collided_pos + _plus_vec);
                Vector2 _end_collide_pos2 = GetEndCollidePosition(_first_collided_pos, _first_collided_pos + _minus_vec);
                if ((_first_collided_pos - _end_collide_pos1).magnitude > (_first_collided_pos - _end_collide_pos2).magnitude)
                {
                    _end_collide_pos1 = _end_collide_pos2;
                }
                List<Vector2> _returnee = GetShortestPath(_end_collide_pos1, _end);
                if (_returnee == null)
                {
                    return null;
                }
                _vec_list.AddRange(_returnee);
                return _vec_list;

            }
        }

        List<MyTile> GetTilesBetweenPosition(Vector2 _start, Vector2 _end, ref Vector2 _last_collided_pos)
        {
            List<MyTile> _tile_list = new List<MyTile>();
            MyTile _dest_tile = GetTileWithActualPos(_end);
            _end = _dest_tile.actualPos;
            Vector2 _unit_vector = _end - _start;
            _unit_vector.Normalize();
            _unit_vector *= MyTile.TILE_Y_SIZE / 5f;
            for (Vector2 _i_vec = _start; (_end - _i_vec).magnitude <= _unit_vector.magnitude; _i_vec += _unit_vector)
            {
                MyTile _tile = GetTileWithActualPos(_i_vec);
                if (!_tile_list.Contains(_tile))
                {
                    if (_tile.colliding == true && _last_collided_pos != Vector2.zero)
                    {
                        _last_collided_pos = _i_vec - _unit_vector;
                    }
                    _tile_list.Add(_tile);
                }

            }
            return _tile_list;
        }

        Vector2 GetEndCollidePosition(Vector2 _start, Vector2 _end)
        {
            List<MyTile> _tile_list = new List<MyTile>();
            Vector2 _first_collided_pos = Vector2.zero;
            MyTile _start_tile = GetTileWithActualPos(_start);
            MyTile _dest_tile = GetTileWithActualPos(_end);
            _start = _start_tile.actualPos;
            _end = _dest_tile.actualPos;
            Vector2 _unit_vector = _end - _start;
            _unit_vector.Normalize();
            _unit_vector *= MyTile.TILE_Y_SIZE / 5f;
            for (Vector2 _i_vec = _start; (_end - _i_vec).magnitude <= _unit_vector.magnitude; _i_vec += _unit_vector)
            {
                MyTile _tile = GetTileWithActualPos(_i_vec);
                if (!_tile_list.Contains(_tile))
                {
                    if (_tile.colliding == false)
                    {
                        _first_collided_pos = _i_vec;
                        return _i_vec;
                    }
                    _tile_list.Add(_tile);
                }

            }
            return _first_collided_pos;
        }
        Vector2 GetFirstCollidePosition(Vector2 _start, Vector2 _end)
        {
            List<MyTile> _tile_list = new List<MyTile>();
            Vector2 _first_collided_pos = Vector2.zero;
            MyTile _dest_tile = GetTileWithActualPos(_end);
            _end = _dest_tile.actualPos;
            Vector2 _unit_vector = _end - _start;
            _unit_vector.Normalize();
            _unit_vector *= MyTile.TILE_Y_SIZE / 5f;
            for (Vector2 _i_vec = _start; (_end - _i_vec).magnitude <= _unit_vector.magnitude; _i_vec += _unit_vector)
            {
                MyTile _tile = GetTileWithActualPos(_i_vec);
                if (!_tile_list.Contains(_tile))
                {
                    if (_tile.colliding == true)
                    {
                        _first_collided_pos = _i_vec;
                        return _i_vec;
                    }
                    _tile_list.Add(_tile);
                }

            }
            return _first_collided_pos;
        }

        IEnumerator MoveToPath(List<Vector2> _path_list)
        {
            bool _toggle = _path_finding_toggle;
            float _timer = 0;
            int _path_index = 0;
            float _speed_timer = 0;
            float _speed = 0;
            int _speed_index = 0;
            _path_finding = true;
            while (_path_index < _path_list.Count - 1 && _path_finding_toggle == _toggle)
            {
                _speed = _speed_array[_speed_index];
                _timer += Time.deltaTime / _speed;
                _speed_timer += Time.deltaTime;
                _player_object.transform.position = Vector2.Lerp(_path_list[_path_index], _path_list[_path_index + 1], _timer);
                yield return null;
                if (_timer > 1)
                {
                    _timer = 0;
                    _path_index++;
                }
                if (_speed_timer > 0.2f)
                {
                    _speed_timer = 0;
                    _speed_index++;
                    if (_speed_index >= _speed_array.Length)
                    {
                        _speed_index = 0;
                    }
                }
            }
            _path_finding = false;
        }

        private void OnDrawGizmos()
        {
            if (_gizmo_list == null)
            {
                return;
            }
            for (int i = 0; i < _gizmo_list.Count; i++)
            {
                Gizmos.DrawCube(_gizmo_list[i], new Vector3(0.1f, 0.1f, 0));
            }
        }

    }

}