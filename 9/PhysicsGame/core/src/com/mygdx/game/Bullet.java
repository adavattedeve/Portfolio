package com.mygdx.game;

import com.badlogic.gdx.graphics.Texture;
import com.badlogic.gdx.math.Polygon;
import com.badlogic.gdx.math.Vector2;
import com.badlogic.gdx.physics.box2d.BodyDef;
import com.badlogic.gdx.physics.box2d.FixtureDef;
import com.badlogic.gdx.physics.box2d.PolygonShape;
import com.badlogic.gdx.physics.box2d.World;
import com.mygdx.mygameutilities.PhysicsGameObject;
import com.mygdx.mygameutilities.PhysicsState;

/**
 * Created by atter on 29-Jan-17.
 */


public class Bullet extends PhysicsGameObject {

    private float speed = 30.0f;
    private Vector2 velocity;

    public Bullet (float x, float y, float dirX, float dirY, PhysicsState state) {
        super(x, y, state);

        setTex(((PhysicsGame)state.getGame()).laserGreenTex);
        setSize(0.3f);

        velocity = new Vector2(dirX, dirY);
        velocity.nor();

        velocity.scl(speed);
        setRotation(360 - velocity.angle(Vector2.Y));
        getBody().setLinearVelocity(velocity);

    }

    @Override
    public void update(float dt) {
        if (getX() < 0 - getSizeX() || getX() > PhysicsGame.WORLD_WIDTH + getSizeX() ||
                getY() < 0 - getSizeY() ||
                getY() > PhysicsGame.WORLD_START_Y + PhysicsGame.WORLD_HEIGHT + getSizeY()) {
            dispose();
        }
    }

    @Override
    public void onCollision(PhysicsGameObject other) {
        if (Meteorite.class.isAssignableFrom(other.getClass())) {
            ((Meteorite)other).takeDamage();
            SmokeEffect temp = new SmokeEffect(getState(), other.getX(), other.getY());
            temp.setPosition(other.getX() - temp.getSizeX() / 2,
                    other.getY() - temp.getSizeY() / 2);
            dispose();
        }
    }

    @Override
    protected BodyDef getDefinitionOfBody(float x, float y) {
        BodyDef bodyDef = new BodyDef();

        bodyDef.type = BodyDef.BodyType.KinematicBody;
        bodyDef.position.set(x, y);
        return bodyDef;
    }

    @Override
    protected FixtureDef getFixtureDefinition() {
        FixtureDef fixture = new FixtureDef();

        PolygonShape box = new PolygonShape();
        box.setAsBox(getSizeX() / 2, getSizeY() / 2);
        fixture.shape = box;
        fixture.isSensor = true;
        return fixture;
    }

    @Override
    protected int getGameObjectLayer() {
        return 0  ;
    }
}
