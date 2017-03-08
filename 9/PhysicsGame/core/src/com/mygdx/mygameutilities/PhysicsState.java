package com.mygdx.mygameutilities;

import com.badlogic.gdx.graphics.g2d.SpriteBatch;
import com.badlogic.gdx.physics.box2d.Body;
import com.badlogic.gdx.physics.box2d.Box2DDebugRenderer;
import com.badlogic.gdx.physics.box2d.ContactListener;
import com.badlogic.gdx.physics.box2d.World;
import com.badlogic.gdx.utils.Array;
import com.mygdx.game.Boundaries;

import java.util.ArrayList;

/**
 * Created by atter on 28-Feb-17.
 */

public abstract class PhysicsState extends State {
    public boolean debugPhysics = true;
    protected World world;
    private PhysicsParameters physicsParams;
    private Box2DDebugRenderer debugRenderer;

    private ArrayList<PhysicsGameObject> stepCallbackRequests;
    private ArrayList<Body> toBeDisposed;

    private double timeStepAccumulator = 0;

    public PhysicsState(GameStateManager gsm, MyGame game) {
        this(gsm, game, new PhysicsParameters());
    }

    public PhysicsState(GameStateManager gsm, MyGame game, PhysicsParameters params) {
        super(gsm, game);
        physicsParams = params;
        world = new World(physicsParams.gravity, true);
        world.setContactListener(getContactListener());
        debugRenderer = new Box2DDebugRenderer();

        stepCallbackRequests = new ArrayList<PhysicsGameObject>();
        toBeDisposed = new ArrayList<Body>();
    }

    @Override
    public void render(SpriteBatch batch) {
        super.render(batch);
        if (debugPhysics) {
            debugRenderer.render(world, cam.combined);
        }
    }

    @Override
    public void disposeState() {
        super.disposeState();
        disposeDisposableBodies();
        world.dispose();
        world = null;
    }

    @Override
    public void updateState(float dt) {
        super.updateState(dt);
        if (world == null)
            return;

        physicsStep(dt);

        while (!stepCallbackRequests.isEmpty()) {
            stepCallbackRequests.get(0).requestedCallbackAfterPhysicsStep();
            stepCallbackRequests.remove(0);
        }
        disposeDisposableBodies();
    }

    public void requestWorldStepCallback(PhysicsGameObject go) {
        stepCallbackRequests.add(go);
    }

    public void destroyBody(Body body) {
        toBeDisposed.add(body);
    }

    protected abstract ContactListener getContactListener();

    private void physicsStep (float dt) {
        float timeStep = physicsParams.timeStep * MyGame.getTimeScale();
        float maxFrameTime = physicsParams.maxFrameTime * MyGame.getTimeScale();

        float frameTime = dt;
        frameTime = frameTime > maxFrameTime ? maxFrameTime : frameTime;

        timeStepAccumulator += frameTime;

        while (timeStepAccumulator >= timeStep) {
            world.step(timeStep,
                    physicsParams.velocityIterations,
                    physicsParams.positionIterations);
            timeStepAccumulator -= timeStep;
        }
    }

    private void disposeDisposableBodies() {
        while (!toBeDisposed.isEmpty()) {
            world.destroyBody(toBeDisposed.get(0));
            toBeDisposed.remove(0);
        }
    }
}
