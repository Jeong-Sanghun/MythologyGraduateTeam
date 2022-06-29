using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttackRange : MonoBehaviour
{
    [SerializeField]
    private GameObject _player;
    [SerializeField]
    private ClientPlayerMove _playerMove;
    [SerializeField]
    private GameObject _tilePref;
    [SerializeField]
    private Transform _tileParent;
    Vector2 _playerPos;
    Vector2 _playerLookPos;
    Vector2 newVectorUp;
    Vector2 newVectorDown;
    List<MyTile> _rangeTileList;
    float timer = 0;

    public void PlayerRangeCheck(float _distance, float _theta)
    {
        //���ۺ��ʹ� �÷��̾ ���� ����. ���̴� ������. ������ ���� ������ ���� �������� ������ +, - ���̱�
        //Vector3 seeVectoer = (_player.transform.forward * _radius);
        //�� ���� �ȿ� ������ Ÿ���� üũ�ؼ� �� Ÿ�ϵ��� ���������� �ٲٱ�, �̰� ������Ʈ Ǯ������
        //���� Ÿ�� �ȿ� �߸� ������Ʈ�� ���� ��� ��ȣ�ۿ� ����
        float alphaX;
        float _radian = _theta * Mathf.PI / 180;
        _playerPos = _player.transform.position;
        _playerLookPos = _player.transform.forward;
        _playerLookPos = new Vector2(_player.transform.forward.x, _player.transform.forward.z);
        _playerLookPos.Normalize();
        if (_playerLookPos.y >= 0)
        {
            alphaX = Mathf.Acos(_playerLookPos.x);
        }
        else
        {
            alphaX = -Mathf.Acos(_playerLookPos.x);
        }

        float newXUp = Mathf.Cos(alphaX + _radian);
        float newYUp = Mathf.Sin(alphaX + _radian);
        newVectorUp = new Vector2(newXUp, newYUp);
        float newXDown = Mathf.Cos(alphaX - _radian);
        float newYDown = Mathf.Sin(alphaX - _radian);
        newVectorDown = new Vector2(newXDown, newYDown);
        _playerLookPos *= _distance;
        newVectorUp *= _distance;
        newVectorDown *= _distance;

        _rangeTileList = _playerMove.GetIntersectingTiles(_playerPos + newVectorDown, _playerPos + newVectorUp);

    }

    public void InstantAttackRange()
    {
        if (Input.GetMouseButtonDown(0))
        {
            for (int i = 0; i < _rangeTileList.Count; i++)
            {
                GameObject _rangeTileObject = Instantiate(_tilePref, _tileParent);
                _rangeTileObject.transform.position = _rangeTileList[i].actualPos;
                _rangeTileObject.transform.position += new Vector3(0.0f, 0.0f, -1.0f);
                Destroy(_rangeTileObject, 1.0f);
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.black;
        Gizmos.DrawLine(_playerPos, _playerPos + _playerLookPos);
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(_playerPos, _playerPos + newVectorUp);
        Gizmos.DrawLine(_playerPos, _playerPos + newVectorDown);
    }

    //������ƮǮ��
    private void ObjectPolling()
    {

    }


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        PlayerRangeCheck(2f, 45f);
        InstantAttackRange();
    }
}
