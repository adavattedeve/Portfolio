package com.mygdx.mygameutilities;

import com.badlogic.gdx.math.Vector2;

/**
 * Created by atter on 28-Feb-17.
 */

public class PhysicsParameters {
    public float timeStep = 1 / 60f;
    public float maxFrameTime = 1 / 4f;
    public Vector2 gravity = new Vector2(0.0f, -9.81f);
    public int velocityIterations = 8;
    public int positionIterations = 3;

    public PhysicsParameters() {

    }

    public PhysicsParameters(float timeStep, float maxFrameTime, Vector2 gravity,
                             int velocityIterations, int positionIterations) {
        this.timeStep = timeStep;
        this.maxFrameTime = maxFrameTime;
        this.gravity = gravity;
        this.velocityIterations = velocityIterations;
        this.positionIterations = positionIterations;
    }
}
