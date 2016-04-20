using UnityEngine;
using System.Collections;


public class PlayerDetails
{
	public System.Action<PlayerDetails> Died;
	private string _playerID;
	private Vector3 _currentPosition;
	private Vector3 _targetPosition;
	private float _size;
	private float _depth;
	private float _moveSpeed;
	private float _rotateSpeed;
	private int _life;
	private bool _alive = false;
	private float _radius;
	private float _rotation;
	private bool _move = false;
	private bool _isStar = false;

	public string PlayerID {
		get {
			return _playerID;
		}
		set {
			_playerID = value;
		}
	}

	public Vector3 CurrentPosition {
		get {
			return _currentPosition;
		}
		set {
			_currentPosition = value;
		}
	}

	public Vector3 TargetPosition {
		get {
			return _targetPosition;
		}
		set {
			_targetPosition = value;
		}
	}

	public float Size {
		get {
			return _size;
		}
		set {
			_size = value;
			_depth = -(_size / 100);
		}
	}

	public float Depth {
		get {
			return _depth;
		}
	}

	public float MoveSpeed {
		get {
			return _moveSpeed;
		}
		set { 
			_moveSpeed = value;
		}
	}

	public float RotateSpeed {
		get {
			return _rotateSpeed;
		}
		set {
			_rotateSpeed = value;
		}
	}

	public int Life {
		get {
			return _life;
		}
		set {
			_life = value;
			if (_life <= 0) {
				Alive = false;
				if (Died != null) {
					Died (this);
				}
			}
		}
	}

	public bool Alive {
		get {
			return _alive;
		}
		set {
			_alive = value;
		}
	}

	public float Radius {
		get {
			return _radius;
		}
		set {
			_radius = value;
		}
	}

	public float Rotation {
		get {
			return _rotation;
		}
		set {
			_rotation = value;
		}
	}

	public bool Move {
		get {
			return _move;
		}
		set {
			_move = value;
		}
	}

	public bool IsStar {
		get {
			return _isStar;
		}
		set {
			_isStar = value;
		}
	}

	public PlayerDetails ()
	{
		Life = 5;
		_playerID = System.Guid.NewGuid ().ToString ();
		Size = 1.0f;
		_alive = false;
		MoveSpeed = 500.0f;
		RotateSpeed = 5.0f;
	}
}

