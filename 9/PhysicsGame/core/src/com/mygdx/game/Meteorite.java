package com.mygdx.game;

import com.badlogic.gdx.audio.Sound;
import com.badlogic.gdx.math.MathUtils;
import com.badlogic.gdx.math.Vector2;
import com.badlogic.gdx.physics.box2d.BodyDef;
import com.badlogic.gdx.physics.box2d.CircleShape;
import com.badlogic.gdx.physics.box2d.FixtureDef;
import com.mygdx.mygameutilities.PhysicsGameObject;
import com.mygdx.mygameutilities.PhysicsState;

/**
 * Created by atter on 29-Jan-17.
 */


public abstract class Meteorite extends PhysicsGameObject {

    protected int scoreValue = 1;
    protected int maxHealth = 1;
    protected float minSpeed = 1f;
    protected float movementForce = 50;
    protected float density = 100f;

    private final float MIN_ANGULAR_VELOCITY = 1;
    private final float MAX_ANGULAR_VELOCITY = 7;

    private int health;

    protected Sound wallCollisionSound;
    protected Sound takeDamageSound;
    protected Sound collideOtherMeteoriteSound;
    protected Sound destructionSound;

    public Meteorite(PhysicsState state) {
        this(0, 0, state);
        float x, y = 0;

        float minX, maxX, minY, maxY;
        minX = getSizeX() / 2;
        maxX = PhysicsGame.WORLD_WIDTH - getSizeX() / 2;
        minY = PhysicsGame.WORLD_START_Y + getSizeY() / 2;
        maxY = PhysicsGame.WORLD_START_Y + PhysicsGame.WORLD_HEIGHT - getSizeX() / 2;

        if (Math.random() < 0.5d) {
            x = (float)Math.random() * (maxX - minX) + minX;
            if (Math.random() < 0.5d) {
                y = maxY;
            } else {
                y = minY;
            }
        } else {
            y = (float)Math.random() * (maxY - minY) + minY;
            if (Math.random() < 0.5d) {
                x = maxX;
            } else {
                x = minX;
            }
        }
        setX(x);
        setY(y);
    }
    public Meteorite(float x, float y, PhysicsState state) {
        super(x, y, state);
        initializeValues();
        movementForce *= density;
        health = maxHealth;

        wallCollisionSound = ((PhysicsGame)getState().getGame()).meteorWallSound;
        takeDamageSound = ((PhysicsGame)getState().getGame()).meteorDamageSound;
        collideOtherMeteoriteSound = ((PhysicsGame)getState().getGame()).meteorMeteorSound;
        destructionSound = ((PhysicsGame)getState().getGame()).meteorDestructionSound;

        Vector2 direction = new Vector2(0, 0);
        direction = direction.setToRandomDirection().nor();

        getBody().applyForce(direction.scl(movementForce),
                new Vector2(getX(), getY()), true);
        setRandomAngularVelocity();
        getBody().setAngularDamping(0.1f);
    }

    public void takeDamage() {
        --health;
        if (health <= 0) {
            destructionSound.play();
            ((PhysicsState)getState()).requestWorldStepCallback(this);
            ((PlayState)getState()).addScores(scoreValue);
            dispose();
        } else {
            destructionSound.play(1f, 0.7f, 0);
            setRandomAngularVelocity();
        }
    }

    @Override
    public void onCollision(PhysicsGameObject other) {
        if (Meteorite.class.isAssignableFrom(other.getClass())) {
            collideOtherMeteoriteSound.play();
        } else if (Boundaries.class.isAssignableFrom(other.getClass())) {
            wallCollisionSound.play();
        }
    }

    @Override
    public void requestedCallbackAfterPhysicsStep() {
        splitIntoSmallerMeteorites();
    }

    @Override
    public void update(float dt) {
        float speed = getBody().getLinearVelocity().len();
        if (speed < minSpeed) {
            getBody().applyForce(getBody().getLinearVelocity().nor().scl(movementForce),
                    new Vector2(getX(), getY()), true);
        }
        getBody().setLinearDamping(MathUtils.lerp(0, 1, speed / minSpeed - 1));

        if (MathUtils.isZero(getBody().getLinearVelocity().x)) {
            Vector2 force = new Vector2(getBody().getLinearVelocity().y * movementForce,
                    0);
            getBody().applyForce(force,
                    new Vector2(getX(), getY()), true);
        }
        if (MathUtils.isZero(getBody().getLinearVelocity().y)) {
            Vector2 force = new Vector2(0,
                    getBody().getLinearVelocity().x * movementForce);
            getBody().applyForce(force,
                    new Vector2(getX(), getY()), true);
        }
    }

    @Override
    public void dispose() {
        super.dispose();
    }

    protected BodyDef getDefinitionOfBody(float x, float y) {
        BodyDef bodyDef = new BodyDef();

        bodyDef.type = BodyDef.BodyType.DynamicBody;
        bodyDef.position.set(x, y);
        return bodyDef;
    }

    protected FixtureDef getFixtureDefinition() {
        FixtureDef fixture = new FixtureDef();

        fixture.density = density;
        fixture.restitution = 1.0f;
        fixture.friction = 0.0f;

        CircleShape circle = new CircleShape();
        circle.setRadius(getSize() / 2);
        fixture.shape = circle;
        return fixture;
    }

    protected abstract void splitIntoSmallerMeteorites();
    protected abstract void initializeValues();

    @Override
    protected int getGameObjectLayer() {
        return 0;
    }

    private void setRandomAngularVelocity() {
        float angularVelocity = (float)Math.random() * (MAX_ANGULAR_VELOCITY - MIN_ANGULAR_VELOCITY)
                + MIN_ANGULAR_VELOCITY;
        if (Math.random() < 0.5d) {
            angularVelocity *= -1;
        }

        getBody().setAngularVelocity(angularVelocity);
    }
}
