using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

namespace TarodevController
{
    /// <summary>
    /// Hey!
    /// Tarodev here. I built this controller as there was a severe lack of quality & free 2D controllers out there.
    /// Right now it only contains movement and jumping, but it should be pretty easy to expand... I may even do it myself
    /// if there's enough interest. You can play and compete for best times here: https://tarodev.itch.io/
    /// If you hve any questions or would like to brag about your score, come to discord: https://discord.gg/GqeHHnhHpz
    /// </summary>
    public class PlayerController : MonoBehaviour, IPlayerController
    {
        // Public for external hooks
        public Vector3 Velocity { get; private set; }
        public FrameInput Input { get; private set; }
        public bool JumpingThisFrame { get; private set; }
        public bool LandingThisFrame { get; private set; }
        public Vector3 RawMovement { get; private set; }
        public bool Grounded => _colDown || _colDanmakuGround;

        private Vector3 _lastPosition;
        private float _currentHorizontalSpeed, _currentVerticalSpeed;

        public GameObject sceneTransitionAnimator;

        public GameObject playerVisual;

        // This is horrible, but for some reason colliders are not fully established when update starts...
        private bool _active;

        private bool _canControl = true;

        void Awake()
        {
            Invoke(nameof(Activate), 0.5f);
            _mainCamera = Camera.main;
        }

        void Activate() => _active = true;

        private void Update()
        {
            if (!_active) return;
            // Calculate velocity
            Velocity = (transform.position - _lastPosition) / Time.deltaTime;
            _lastPosition = transform.position;

            GatherInput();
            CalculateRayRanged();
            RunEnvironmentCollisionChecks();
            RunDanmakuCollisionChecks();
            RunBulletCollisionChecks();
            RunLivingEnemyCollisionChecks();
            RunNonLivingEnemyCollisionChecks();

            BounceBullet();

            DeathChecks();

            if (_canControl) CalculateWalk(); // Horizontal movement
            CalculateAttack();
            CalculateDash();
            CalculateJumpApex(); // Affects fall speed, so calculate before gravity
            CalculateGravity(); // Vertical movement
            CalculateJump(); // Possibly overrides vertical

            // OnDrawGizmos();
            MoveCharacter(); // Actually perform the axis movement

            if (transform.position.y < -50) sceneTransitionAnimator.GetComponent<SceneTransitionController>().SetTransition(SceneManager.GetActiveScene().name);
        }


        #region Gather Input

        private void GatherInput()
        {
            Input = new FrameInput
            {
                JumpDown = UnityEngine.Input.GetButtonDown("Jump"),
                JumpUp = UnityEngine.Input.GetButtonUp("Jump"),
                X = UnityEngine.Input.GetAxisRaw("Horizontal"),
                Y = UnityEngine.Input.GetAxisRaw("Vertical")
            };
            if (Input.JumpDown)
            {
                _lastJumpPressed = Time.time;
            }
        }

        #endregion

        #region Collisions

        [Header("COLLISION")][SerializeField] private bool enableDanmaku = true;
        [SerializeField] private Bounds _characterBounds;
        [SerializeField] private LayerMask _groundLayer;
        [SerializeField] private LayerMask _danmakuLayer;
        [SerializeField] private LayerMask _bulletLayer;
        [SerializeField] private LayerMask _livingEnemyLayer;
        [SerializeField] private LayerMask _nonLivingEnemyLayer;
        [SerializeField] private int _detectorCount = 3;
        [SerializeField] private float _detectionRayLength = 0.1f;
        [SerializeField][Range(0.1f, 0.3f)] private float _rayBuffer = 0.1f; // Prevents side detectors hitting the ground

        private RayRange _raysUp, _raysRight, _raysDown, _raysLeft;
        private bool _colUp, _colRight, _colDown, _colLeft;
        private bool _colDanmaku, _colDanmakuGround;
        private bool _colBullet;
        private bool _colLivingEnemy;
        private bool _colNonLivingEnemy;

        private float _timeLeftGrounded;

        // We use these raycast checks for pre-collision information
        private void RunEnvironmentCollisionChecks()
        {
            // Ground
            LandingThisFrame = false;
            var groundedCheck = RunDetection(_raysDown);
            switch (_colDown)
            {
                case true when !groundedCheck:
                    _timeLeftGrounded = Time.time; // Only trigger when first leaving
                    break;
                case false when groundedCheck:
                    _coyoteUsable = true; // Only trigger when first touching
                    LandingThisFrame = true;
                    break;
            }

            _colDown = groundedCheck;

            // The rest
            _colUp = RunDetection(_raysUp);
            _colLeft = RunDetection(_raysLeft);
            _colRight = RunDetection(_raysRight);

            if (_colLeft || _colRight || _colUp)
            {
                _consumeDashToWall = false;
            }

            bool RunDetection(RayRange range)
            {
                return EvaluateRayPositions(range).Any(point => Physics2D.Raycast(point, range.Dir, _detectionRayLength, _groundLayer));
            }
        }

        private void RunNonLivingEnemyCollisionChecks()
        {

            // The rest
            _colNonLivingEnemy = RunDetection(_raysUp) || RunDetection(_raysDown) || RunDetection(_raysLeft) ||
                              RunDetection(_raysRight);

            if (_colNonLivingEnemy) _dead = true;

            bool RunDetection(RayRange range)
            {
                return EvaluateRayPositions(range).Any(point => Physics2D.Raycast(point, range.Dir, _detectionRayLength, _nonLivingEnemyLayer));
            }
        }

        private void RunLivingEnemyCollisionChecks()
        {
            _colLivingEnemy = RunDetection(_raysUp) || RunDetection(_raysLeft) || RunDetection(_raysRight) ||
                        RunDetection(_raysDown);

            bool RunDetection(RayRange range)
            {
                foreach (var point in EvaluateRayPositions(range))
                {
                    var hit = Physics2D.Raycast(point, range.Dir, _detectionRayLength, _livingEnemyLayer);
                    if (!hit) continue;
                    if (!_isAttacking)
                    {
                        _dead = true;
                        return hit;
                    }
                    var enemy = hit.collider.gameObject;
                    Destroy(enemy);
                    return hit;
                }
                return false;
            }
        }

        private void RunDanmakuCollisionChecks()
        {
            if (!enableDanmaku) return;
            // Ground
            LandingThisFrame = false;
            var groundedCheck = RunDetection(_raysDown);
            switch (_colDanmakuGround)
            {
                case true when !groundedCheck:
                    _timeLeftGrounded = Time.time; // Only trigger when first leaving
                    break;
                case false when groundedCheck:
                    _coyoteUsable = true; // Only trigger when first touching
                    LandingThisFrame = true;
                    break;
            }

            _colDanmakuGround = groundedCheck;

            _colDanmaku = RunDetection(_raysUp) || RunDetection(_raysLeft) || RunDetection(_raysRight);

            if (_colDanmakuGround) _colDanmaku = false;

            bool RunDetection(RayRange range)
            {
                foreach (var point in EvaluateRayPositions(range))
                {
                    var hit = Physics2D.Raycast(point, range.Dir, _detectionRayLength, _danmakuLayer);
                    if (!hit) continue;
                    var danmaku = hit.collider.gameObject;
                    var danmakuController = danmaku.GetComponent<Danmaku>();
                    danmakuController.destroy = true;
                    if (range.Start != _raysDown.Start && range.Dir != _raysDown.Dir && range.End != _raysDown.End)
                    {
                        HandleDanmakuEffects(danmakuController.danmakuTypes);
                    }
                    return hit;
                }
                return false;
            }
        }

        private void RunBulletCollisionChecks()
        {
            if (!bounceEnabled) return;
            _colBullet = RunDetection(_raysUp) || RunDetection(_raysLeft) || RunDetection(_raysRight) || RunDetection(_raysDown);
            if (_colBullet)
            {
                _dead = true;
            }

            bool RunDetection(RayRange range)
            {
                foreach (var point in EvaluateRayPositions(range))
                {
                    var hit = Physics2D.Raycast(point, range.Dir, _detectionRayLength, _bulletLayer);
                    if (!hit) continue;
                    return hit;
                }
                return false;
            }
        }

        private bool _dead;

        private void DeathChecks()
        {
            if (!_dead) return;
            _dead = false;
            sceneTransitionAnimator
                .GetComponent<SceneTransitionController>()
                .SetTransition(SceneManager.GetActiveScene().name);
        }

        private void CalculateRayRanged()
        {
            // This is crying out for some kind of refactor. 
            var b = new Bounds(transform.position, _characterBounds.size);

            _raysDown = new RayRange(b.min.x + _rayBuffer, b.min.y, b.max.x - _rayBuffer, b.min.y, Vector2.down);
            _raysUp = new RayRange(b.min.x + _rayBuffer, b.max.y, b.max.x - _rayBuffer, b.max.y, Vector2.up);
            _raysLeft = new RayRange(b.min.x, b.min.y + _rayBuffer, b.min.x, b.max.y - _rayBuffer, Vector2.left);
            _raysRight = new RayRange(b.max.x, b.min.y + _rayBuffer, b.max.x, b.max.y - _rayBuffer, Vector2.right);
        }


        private IEnumerable<Vector2> EvaluateRayPositions(RayRange range)
        {
            for (var i = 0; i < _detectorCount; i++)
            {
                var t = (float)i / (_detectorCount - 1);
                yield return Vector2.Lerp(range.Start, range.End, t);
            }
        }

        private void OnDrawGizmos()
        {
            // Bounds
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube(transform.position + _characterBounds.center, _characterBounds.size);

            // Rays
            if (!Application.isPlaying)
            {
                CalculateRayRanged();
                Gizmos.color = Color.blue;
                foreach (var range in new List<RayRange> { _raysUp, _raysRight, _raysDown, _raysLeft })
                {
                    foreach (var point in EvaluateRayPositions(range))
                    {
                        Gizmos.DrawRay(point, range.Dir * _detectionRayLength);
                    }
                }
            }

            if (!Application.isPlaying) return;

            // Draw the future position. Handy for visualizing gravity
            Gizmos.color = Color.red;
            var move = new Vector3(_currentHorizontalSpeed, _currentVerticalSpeed) * Time.deltaTime;
            Gizmos.DrawWireCube(transform.position + move, _characterBounds.size);
        }

        #endregion

        #region Attack

        [Header("ATTACK")]
        [SerializeField] private float attackTime;
        [SerializeField] private float attackSpeed;
        [SerializeField] private bool enableAttack;
        [SerializeField] private float attackCooldown;
        [SerializeField] private float angleBeared;
        [SerializeField] private float radius;

        private bool _isAttacking;
        private float _attackDeltaTime;
        private float _attackPassedTime;
        private float _cooldownDeltaTime;
        private bool _lastAttackLasting;
        private Vector3 _targetPosition;
        private Vector2 direction;

        public int SoftMax(List<float> mike)
        {
            float lastEpoch = 0;
            for (int c = 0; c < mike.Count; c++)
            {
                if (c == 0) lastEpoch = mike[c];
                lastEpoch = mike[c] > lastEpoch ?
                mike[c]
                :
                lastEpoch;
            }
            return mike.IndexOf(lastEpoch);
        }
        //public double Angle(Vector3 cen, Vector3 first, Vector3 second)
        //{
        //    double ma_x = first.x - cen.x;
        //    double ma_y = first.y - cen.y;
        //    double mb_x = second.x - cen.x;
        //    double mb_y = second.y - cen.y;
        //    double v1 = (ma_x * mb_x) + (ma_y * mb_y);
        //    double ma_val = Math.Sqrt(ma_x * ma_x + ma_y * ma_y);
        //    double mb_val = Math.Sqrt(mb_x * mb_x + mb_y * mb_y);
        //    double cosM = v1 / (ma_val * mb_val);
        //    double angleAMB = Math.Acos(cosM) * 180 / Mathf.PI;
        //    return angleAMB;
        //}
        private void CalculateAttack()
        {
            if (!enableAttack) return;
            if (_attackDeltaTime >= attackTime)
            {
                _attackDeltaTime = 0;
                _isAttacking = false;
            }

            if (UnityEngine.Input.GetKeyDown(KeyCode.Mouse0) && _cooldownDeltaTime >= attackCooldown)
            {

                Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, radius, _livingEnemyLayer);
                direction = colliders
                    .Select(x => x.transform)
                    .Where(x => Vector2.Angle(x.position - transform.position, _mainCamera.ScreenToWorldPoint(UnityEngine.Input.mousePosition)
                    - transform.position) < angleBeared)
                    .OrderBy(x => Vector2.Distance(x.position, transform.position))
                    .Select(x => Math.Direction(x.position - transform.position))
                    .FirstOrDefault();

                if (direction == null)
                {
                    direction = new Vector2(0, 0);
                }
                _canControl = false;

                //int num = 0;
                //for (int i = 0;i < ObjectsTransforms.Count; i++)
                //{
                //    double angle = Vector2.Angle(ObjectsTransforms[i].position-transform.position,
                //        _mainCamera.ScreenToWorldPoint(UnityEngine.Input.mousePosition)-transform.position);
                //    Debug.Log(angle);
                //    if (angle > angleBeared) continue;
                //    Debug.Log(angle);
                //    ObjectsTransforms[num] = colliders[i].transform;
                //    num++;
                //}
                //if (ObjectsTransforms.Count == 0) return;
                //List<float> distance = new List<float>();
                //for(int i = 0;i<ObjectsTransforms.Count;i++)
                //    distance.Add(Vector2.Distance(ObjectsTransforms[i].position, transform.position));
                //Transform ObjectsTransform = ObjectsTransforms[SoftMax(distance)];
                //_isAttacking = true;
                //_cooldownDeltaTime = 0;
                //_targetPosition = ObjectsTransform.position;
                //direction = Math.Direction(_targetPosition - transform.position);
                _isAttacking = true;
                _cooldownDeltaTime = 0;
                playerVisual.transform.localScale = new Vector3(direction.x > 0 ? 1 : -1, 1, 1);
            }
            if (!_isAttacking)
            {
                _cooldownDeltaTime += Time.deltaTime - _attackPassedTime;
                _canControl = true;
                return;
            }

            _attackDeltaTime += Time.deltaTime - _attackPassedTime;
            if (_mainCamera is null) return;

            if (direction != Vector2.zero)
            {
                var velocity = direction * attackSpeed;
                _currentHorizontalSpeed = velocity.x;
                _currentVerticalSpeed = velocity.y;
                if ((!(_currentHorizontalSpeed > 0) || !_colRight)
                && (!(_currentHorizontalSpeed < 0) || !_colLeft)
                && (!(_currentVerticalSpeed > 0) || !_colUp)
                && (!(_currentVerticalSpeed < 0) || !_colDown)) return;
                // Don't walk through walls
                _currentHorizontalSpeed = 0;
                _currentVerticalSpeed = 0;
                return;
            }

            _canControl = true;
        }

        #endregion

        #region Dash

        [Header("DASH")]
        [SerializeField] private float dashTime;
        [SerializeField] private float dashSpeed;
        [SerializeField] private float dashCooldown;
        [SerializeField] private float notMovableInterval;
        [SerializeField] private bool enableDash;

        private bool _isDashing;
        private bool _canDash;
        private float _dashDeltaTime;
        private float _passedTime;
        private bool _consumeDashToWall;

        private void CalculateDash()
        {
            if (!enableDash) return;
            if (_isAttacking) return;
            if (Grounded)
            {
                _canDash = true;
            }

            if (_dashDeltaTime >= dashTime)
            {
                _dashDeltaTime = 0;
                _isDashing = false;
            }
            if (UnityEngine.Input.GetKeyDown(KeyCode.LeftShift) && _canDash || _consumeDashToWall)
            {
                _isDashing = true;
                _canControl = false;
                _canDash = false;
            }

            if (_dashDeltaTime >= notMovableInterval) _canControl = true;

            if (!_isDashing) return;

            _dashDeltaTime += Time.deltaTime - _passedTime;
            var direction = Input is { X: 0, Y: 0 } ?
                new Vector2(playerVisual.transform.localScale.x, 0)
                :
                new Vector2(Input.X, Input.Y).normalized;
            var velocity = direction * dashSpeed;
            _currentHorizontalSpeed = velocity.x;
            _currentVerticalSpeed = velocity.y / 2;
            if ((!(_currentHorizontalSpeed > 0) || !_colRight)
                && (!(_currentHorizontalSpeed < 0) || !_colLeft)
                && (!(_currentVerticalSpeed > 0) || !_colUp)
                && (!(_currentVerticalSpeed < 0) || !_colDown)) return;
            // Don't walk through walls
            _currentHorizontalSpeed = 0;
            _currentVerticalSpeed = 0;
        }

        #endregion


        #region Walk

        [Header("WALKING")][SerializeField] private float _acceleration = 90;
        [SerializeField] private float _moveClamp = 13;
        [SerializeField] private float _deAcceleration = 60f;
        [SerializeField] private float _apexBonus = 2;

        private void CalculateWalk()
        {
            if (Input.X != 0)
            {
                // Set horizontal move speed
                _currentHorizontalSpeed += Input.X * _acceleration * Time.deltaTime;

                // clamped by max frame movement
                _currentHorizontalSpeed = Mathf.Clamp(_currentHorizontalSpeed, -_moveClamp, _moveClamp);

                // Apply bonus at the apex of a jump
                var apexBonus = Mathf.Sign(Input.X) * _apexBonus * _apexPoint;
                _currentHorizontalSpeed += apexBonus * Time.deltaTime;
            }
            else
            {
                // No input. Let's slow the character down
                _currentHorizontalSpeed = Mathf.MoveTowards(_currentHorizontalSpeed, 0, _deAcceleration * Time.deltaTime);
            }

            if (_currentHorizontalSpeed > 0 && _colRight || _currentHorizontalSpeed < 0 && _colLeft)
            {
                // Don't walk through walls
                _currentHorizontalSpeed = 0;
            }
        }

        #endregion

        #region Gravity

        [Header("GRAVITY")][SerializeField] private float _fallClamp = -40f;
        [SerializeField] private float _minFallSpeed = 80f;
        [SerializeField] private float _maxFallSpeed = 120f;
        private float _fallSpeed;

        private void CalculateGravity()
        {
            if (_colDown || _colDanmakuGround)
            {
                // Move out of the ground
                if (_currentVerticalSpeed < 0) _currentVerticalSpeed = 0;
            }
            else
            {
                // Add downward force while ascending if we ended the jump early
                var fallSpeed = _endedJumpEarly && _currentVerticalSpeed > 0 ? _fallSpeed * _jumpEndEarlyGravityModifier : _fallSpeed;

                // Fall
                _currentVerticalSpeed -= fallSpeed * Time.deltaTime;

                // Clamp
                if (_currentVerticalSpeed < _fallClamp) _currentVerticalSpeed = _fallClamp;
            }
        }

        #endregion

        #region Danmaku

        private void HandleDanmakuEffects(List<Danmaku.DanmakuType> effects)
        {
            foreach (var effect in effects)
            {
                switch (effect)
                {
                    case Danmaku.DanmakuType.DisableDash:
                        enableDash = false;
                        break;
                    case Danmaku.DanmakuType.DashUntilColWall:
                        _consumeDashToWall = true;
                        break;
                    case Danmaku.DanmakuType.DisableDoubleJump:
                        _doubleJumpEnabled = false;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        #endregion

        #region Bounce

        [Header("BOUNCE")][SerializeField] private bool bounceEnabled;
        [SerializeField] private float overlapBoxSizeX;
        [SerializeField] private float overlapBoxSizeY;
        [SerializeField] private float overlapBoxOffSetX;
        [SerializeField] private float overlapBoxOffSetY;

        private Camera _mainCamera;

        private void BounceBullet()
        {
            if (!bounceEnabled) return;
            if (!UnityEngine.Input.GetMouseButton(0)) return;
            var bullet = Physics2D.OverlapBox(transform.position + new Vector3(overlapBoxOffSetX, overlapBoxOffSetY, 0), new Vector2(overlapBoxSizeX, overlapBoxSizeY), 0, _bulletLayer);
            if (!bullet) return;
            var bulletController = bullet.GetComponent<Bullet>();
            if (_mainCamera is null) return;
            var mousePosition = _mainCamera.ScreenToWorldPoint(UnityEngine.Input.mousePosition);
            bulletController.direction = (mousePosition - transform.position).normalized;
        }

        #endregion

        #region Jump

        [Header("JUMPING")][SerializeField] private float _jumpHeight = 30;
        [SerializeField] private float _jumpApexThreshold = 10f;
        [SerializeField] private float _coyoteTimeThreshold = 0.1f;
        [SerializeField] private float _jumpBuffer = 0.1f;
        [SerializeField] private float _jumpEndEarlyGravityModifier = 3;
        [SerializeField] private bool _doubleJumpEnabled;
        private bool _coyoteUsable;
        private bool _endedJumpEarly = true;
        private float _apexPoint; // Becomes 1 at the apex of a jump
        private float _lastJumpPressed;
        private bool CanUseCoyote => _coyoteUsable && !_colDown && _timeLeftGrounded + _coyoteTimeThreshold > Time.time;
        private bool HasBufferedJump => _colDown && _lastJumpPressed + _jumpBuffer > Time.time;
        private bool HasDoubleJump => !Grounded && _currentJumpedTime == 1;

        private int _currentJumpedTime;

        private void CalculateJumpApex()
        {
            if (!_colDown)
            {
                // Gets stronger the closer to the top of the jump
                _apexPoint = Mathf.InverseLerp(_jumpApexThreshold, 0, Mathf.Abs(Velocity.y));
                _fallSpeed = Mathf.Lerp(_minFallSpeed, _maxFallSpeed, _apexPoint);
            }
            else
            {
                _apexPoint = 0;
            }
        }

        private void CalculateJump()
        {
            if (!Input.JumpDown && Grounded) _currentJumpedTime = 0;

            // Jump if: grounded or within coyote threshold || sufficient jump buffer
            if (Input.JumpDown && CanUseCoyote || HasBufferedJump || _colDanmakuGround || Input.JumpDown && HasDoubleJump && _doubleJumpEnabled)
            {
                _currentVerticalSpeed = _jumpHeight;
                _endedJumpEarly = false;
                _coyoteUsable = false;
                _timeLeftGrounded = float.MinValue;
                JumpingThisFrame = true;
                _currentJumpedTime++;
            }
            else
            {
                JumpingThisFrame = false;
            }

            // End the jump early if button released
            if (!_colDown && Input.JumpUp && !_endedJumpEarly && Velocity.y > 0)
            {
                // _currentVerticalSpeed = 0;
                _endedJumpEarly = true;
            }

            if (_colUp)
            {
                if (_currentVerticalSpeed > 0) _currentVerticalSpeed = 0;
            }
        }

        #endregion

        #region Move

        [Header("MOVE")]
        [SerializeField, Tooltip("Raising this value increases collision accuracy at the cost of performance.")]
        private int _freeColliderIterations = 10;

        // We cast our bounds before moving to avoid future collisions
        private void MoveCharacter()
        {
            var pos = transform.position;
            RawMovement = new Vector3(_currentHorizontalSpeed, _currentVerticalSpeed); // Used externally
            var move = RawMovement * Time.deltaTime;
            var furthestPoint = pos + move;

            // check furthest movement. If nothing hit, move and don't do extra checks
            var hit = Physics2D.OverlapBox(furthestPoint, _characterBounds.size, 0, _groundLayer);
            if (!hit)
            {
                transform.position += move;
                return;
            }

            // otherwise increment away from current pos; see what closest position we can move to
            var positionToMoveTo = transform.position;
            for (int i = 1; i < _freeColliderIterations; i++)
            {
                // increment to check all but furthestPoint - we did that already
                var t = (float)i / _freeColliderIterations;
                var posToTry = Vector2.Lerp(pos, furthestPoint, t);

                if (Physics2D.OverlapBox(posToTry, _characterBounds.size, 0, _groundLayer))
                {
                    transform.position = positionToMoveTo;

                    // We've landed on a corner or hit our head on a ledge. Nudge the player gently
                    if (i == 1)
                    {
                        if (_currentVerticalSpeed < 0) _currentVerticalSpeed = 0;
                        var dir = transform.position - hit.transform.position;
                        transform.position += dir.normalized * move.magnitude;
                    }

                    return;
                }

                positionToMoveTo = posToTry;
            }
        }

        #endregion
    }
}