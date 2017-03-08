package com.mygdx.mygameutilities;

import com.badlogic.gdx.Gdx;
import com.badlogic.gdx.audio.Music;
import com.badlogic.gdx.graphics.Texture;
import com.badlogic.gdx.graphics.g2d.SpriteBatch;
import com.badlogic.gdx.math.MathUtils;
import com.badlogic.gdx.physics.box2d.Body;
import com.badlogic.gdx.physics.box2d.BodyDef;
import com.badlogic.gdx.physics.box2d.CircleShape;
import com.badlogic.gdx.physics.box2d.FixtureDef;
import com.badlogic.gdx.physics.box2d.World;
import com.mygdx.game.SpaceShip;

/**
 * Created by atter on 28-Feb-17.
 */

public abstract class PhysicsGameObject extends GameObject {

    private Body body;
   // private PhysicsState state;
    private World world;

    public PhysicsGameObject(float x, float y, PhysicsState state) {
        super(state);
        world = state.world;
        body = world.createBody(getDefinitionOfBody(x, y));
        FixtureDef fix = getFixtureDefinition();
        body.setUserData(this);
        if (fix != null) {
            body.createFixture(fix);
            fix.shape.dispose();
        }


        setX(x);
        setY(y);

    }

    @Override
    public final void render(SpriteBatch batch) {
        if (textureRegion == null)
            return;

        batch.draw(textureRegion,
                getX() - getSizeX() / 2,
                getY() - getSizeY() / 2,
                getSizeX() / 2,
                getSizeY() / 2,
                getSizeX(),
                getSizeY(),
                1f,
                1f,
                getRotation()
        );
    }

    @Override
    public float getX() {
        return body.getPosition().x;
    }

    @Override
    public float getY() {
        return body.getPosition().y;
    }

    @Override
    public void setX(float x) {
        getBody().setTransform(x, getY(), getRotation());
    }

    @Override
    public void setY(float y) {
        getBody().setTransform(getX(), y, getRotation());
    }

    @Override
    public float getRotation() {
        return body.getTransform().getRotation() * MathUtils.radiansToDegrees;
    }

    @Override
    public void setRotation(float rotation) {
        super.setRotation(rotation);
        getBody().setTransform(getX(), getY(), super.getRotation() * MathUtils.degreesToRadians);
    }

    public Body getBody() {
        return body;
    }

    public World getWorld() {
        return world;
    }

    public void onCollision(PhysicsGameObject other) {

    }

    public void requestedCallbackAfterPhysicsStep() {

    }

    @Override
    public void dispose() {
        super.dispose();
        ((PhysicsState)getState()).destroyBody(body);
    }

    @Override
    protected void onSizeChange(float oldSize) {
        getBody().destroyFixture(getBody().getFixtureList().get(0));
        getBody().createFixture(getFixtureDefinition());
    }

    protected abstract BodyDef getDefinitionOfBody(float x, float y);

    protected abstract FixtureDef getFixtureDefinition();

}
