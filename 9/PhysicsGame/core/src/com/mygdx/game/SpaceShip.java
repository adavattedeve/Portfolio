package com.mygdx.game;

import com.badlogic.gdx.audio.Sound;
import com.badlogic.gdx.math.MathUtils;
import com.badlogic.gdx.math.Vector2;
import com.badlogic.gdx.physics.box2d.BodyDef;
import com.badlogic.gdx.physics.box2d.FixtureDef;
import com.badlogic.gdx.physics.box2d.PolygonShape;
import com.mygdx.mygameutilities.PhysicsGameObject;
import com.mygdx.mygameutilities.*;

/**
 * Created by atter on 27-Jan-17.
 */

public class SpaceShip extends PhysicsGameObject {


    private int maxHealth = 3;
    private float shootCooldown = 0.4f;
    private float invulnerableDuration = 2.5f;
    private float outOfControlDuration = 0.2f;

    private float maxVelocity = 3.0f;
    private float linearDamping = 2f;
    private float movingForce = 1000f;

    private float minAngularAccerelation = 12f;
    private float maxAngularAccerelation = 20f;

    private float minAngularVelocityCap = 2.3f;
    private float maxAngularVelocityCap = 4.5f;

    private float currentShootCooldown = 0.0f;
    private float angularAccerelation = 0;
    private float angularVelocity = 0;
    private float angularVelocityCap = 0;
    private int health = 0;
    private float invulnerableTimer = 0;
    private float outOfControlTimer = 0;
    private boolean accelerate = false;
    //positive = clockwise, negative = counter clockwise, 0 = not rotating
    private int rotationDirection;
    private boolean destroyed = false;
    private boolean invulnerable = false;

    private Sound shootSound;
    private Sound destructionSound;
    private Sound accelerateSound;

    public SpaceShip(float x, float y, PhysicsState state) {
        super(x, y, state);
        health = maxHealth;
        outOfControlTimer = outOfControlDuration;
        shootSound = ((PhysicsGame)state.getGame()).spaceshipShootSound;
        destructionSound = ((PhysicsGame)state.getGame()).spaceshipDestructionSound;
        accelerateSound = ((PhysicsGame)state.getGame()).spacehipAccerelateSound;

        setTex(((PhysicsGame)state.getGame()).playerTex);

        getBody().setLinearDamping(linearDamping);
        setSize(0.3f);
    }

    @Override
    public void onCollision(PhysicsGameObject other) {
        if (Meteorite.class.isAssignableFrom(other.getClass())) {
            takeDamage();
        }
    }

    @Override
    public void update(float dt) {

        currentShootCooldown += dt;
        if (invulnerable) {
            invulnerableTimer += dt;
            if (invulnerableTimer > invulnerableDuration) {
                invulnerableTimer = 0;
                invulnerable = false;
            }
        }

        if (outOfControlTimer < outOfControlDuration) {
            outOfControlTimer += dt;
            return;
        }

        float currentVelocity = getBody().getLinearVelocity().len();

        angularAccerelation = MathUtils.lerp(maxAngularAccerelation, minAngularAccerelation,
                currentVelocity / maxVelocity);
        angularVelocityCap = MathUtils.lerp(maxAngularVelocityCap, minAngularVelocityCap,
                currentVelocity / maxVelocity);

        if (rotationDirection > 0) {
            angularVelocity += angularAccerelation * dt;
        } else if (rotationDirection < 0) {
            angularVelocity -= angularAccerelation * dt;
        } else if (!MathUtils.isZero(angularVelocity)) {

            float delta = Math.signum(angularVelocity) * angularAccerelation * dt;

            if (Math.abs(angularVelocity) < Math.abs(delta)) {
                angularVelocity = 0;
            } else {
                angularVelocity -= delta;
            }
        }

        angularVelocity = MathUtils.clamp(angularVelocity, -angularVelocityCap, angularVelocityCap);

        getBody().setAngularVelocity(angularVelocity);

        Vector2 forward = new Vector2((float)Math.cos(Math.toRadians(getRotation() + 90)),
                (float)Math.sin(Math.toRadians(getRotation() + 90)));

        if (accelerate && currentVelocity < maxVelocity) {
            getBody().applyForce(movingForce * dt * forward.x,
                    movingForce * dt * forward.y,
                    getX() - forward.x * getSizeX(),
                    getY() - forward.y * getSizeY(), true);
        }

    }

    public void shoot() {

        if (currentShootCooldown < shootCooldown || outOfControlTimer < outOfControlDuration) {
            return;
        }

        Vector2 forward = new Vector2((float)Math.cos(Math.toRadians(getRotation() + 90)),
                (float)Math.sin(Math.toRadians(getRotation() + 90)));

        currentShootCooldown = 0.0f;

        new Bullet(getX() + forward.x * getSizeY(),
                getY() + forward.y * getSizeY(),
                forward.x,
                forward.y, ((PhysicsState)getState()));
        shootSound.play();
    }


    public void setAccelerate (boolean accelerate) {

        if (!this.accelerate && accelerate) {
            accelerateSound.loop();
        } else if (this.accelerate && !accelerate) {
            accelerateSound.stop();
        }
        this.accelerate = accelerate;
    }

    public void setRotationDirection (int rotationDirection) {
        this.rotationDirection = rotationDirection;
    }

    public boolean isDestroyed() {
        return destroyed;
    }

    public int getHealth() {
        return health;
    }

    @Override
    protected BodyDef getDefinitionOfBody(float x, float y) {
        BodyDef bodyDef = new BodyDef();

        bodyDef.type = BodyDef.BodyType.DynamicBody;
        bodyDef.position.set(x, y);

        return bodyDef;
    }

    @Override
    protected FixtureDef getFixtureDefinition() {
        FixtureDef fixture = new FixtureDef();

        PolygonShape box = new PolygonShape();
        box.setAsBox(getSizeX() / 2, getSizeY() / 2);
        fixture.shape = box;
        fixture.density = 0;
        fixture.restitution = 1.0f;
        fixture.friction = 0.0f;

        return fixture;
    }

    @Override
    protected int getGameObjectLayer() {
        return 1;
    }

    private void takeDamage() {
        if (invulnerable)
            return;
        health--;
        if (health > 0) {
            MyGame.shakeCamera(0.15f, 0.08f);
            destructionSound.play(0.5f);
            invulnerable = true;
            TimeScaleManipulator.slowMotion(0.1f, 1f, 10);
            getBody().setAngularVelocity(50);
            outOfControlTimer = 0;
        } else {
            ExplosionEffect temp = new ExplosionEffect(getState(), getX(), getY());
            temp.setPosition(getX() - temp.getSizeX() / 2,
                    getY() - temp.getSizeY() / 2);
            MyGame.shakeCamera(0.2f, 0.1f);
            destructionSound.play(1f);
            setAccelerate(false);
            destroyed = true;
            dispose();
        }
    }
}