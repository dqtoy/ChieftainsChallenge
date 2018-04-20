﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class GameBall : MonoBehaviour {

    private Rigidbody2D m_RigidBody;
    private Coroutine m_BallSleepRoutine;
    private float m_BallCollisionStart = 0;

    private Vector3 m_MouseClickStart;

    private const float VELOCITY_SLEEP_THRESHOLD = 0.05f;

    private void Awake() {
        m_RigidBody = GetComponent<Rigidbody2D>();
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.Space)) {
            HitBall(new Vector2(1, 1), 5);
        }

        if (Input.GetMouseButtonDown(0)) {
            m_MouseClickStart = Input.mousePosition;
        }

        if (Input.GetMouseButtonUp(0)) {
            Vector3 delta = Input.mousePosition - m_MouseClickStart;
            HitBall(-delta, delta.magnitude / 10);
        }
    }

    public void HitBall(Vector2 direction, float power) {
        StopSleepRoutine();

        m_RigidBody.AddForce(direction.normalized * power, ForceMode2D.Impulse);
    }

    #region Ball sleeping
    private void OnCollisionEnter2D(Collision2D collision) {
        m_BallCollisionStart = Time.time;
    }

    private void OnCollisionStay2D(Collision2D collision) {
        m_RigidBody.velocity *= (1f - (Time.time - m_BallCollisionStart) * 0.05f);

        //Start a routine to check if the ball will stay still
        if (m_RigidBody.velocity.magnitude <= VELOCITY_SLEEP_THRESHOLD) {
            StartSleepRoutine();
        } else {
            StopSleepRoutine();
        }
    }

    private void OnCollisionExit2D(Collision2D collision) {
        StopSleepRoutine();
    }

    private void StartSleepRoutine() {
        if (m_BallSleepRoutine == null) m_BallSleepRoutine = StartCoroutine(BallSleepRoutine());
    }

    private void StopSleepRoutine() {
        if (m_BallSleepRoutine != null) {
            StopCoroutine(m_BallSleepRoutine);
            m_BallSleepRoutine = null;
        }
    }

    private IEnumerator BallSleepRoutine() {
        float startTime = Time.time;
        float waitTime = 2;

        yield return new WaitForSeconds(waitTime);

        if(m_RigidBody.velocity.magnitude <= VELOCITY_SLEEP_THRESHOLD) {
            while (true) {
                m_RigidBody.velocity = Vector2.zero;
                yield return null;
            }
        }

        m_BallSleepRoutine = null;
    }
    #endregion
}