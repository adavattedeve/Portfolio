package com.mygdx.game;

import com.badlogic.gdx.math.Vector2;
import com.badlogic.gdx.physics.box2d.BodyDef;
import com.badlogic.gdx.physics.box2d.FixtureDef;
import com.badlogic.gdx.physics.box2d.PolygonShape;
import com.badlogic.gdx.physics.box2d.World;
import com.mygdx.mygameutilities.PhysicsGameObject;
import com.mygdx.mygameutilities.PhysicsState;

/**
 * Created by atter on 01-Mar-17.
 */

public class Boundaries extends PhysicsGameObject {

    private float xSize;
    private float ySize;

    public Boundaries(float x, float y, float xSize, float ySize, PhysicsState state) {
        super(x, y, state);
        this.xSize = xSize;
        this.ySize = ySize;
        setFixtures();
    }
    @Override
    protected BodyDef getDefinitionOfBody(float x, float y) {
        BodyDef bodyDef = new BodyDef();

        bodyDef.type = BodyDef.BodyType.StaticBody;
        bodyDef.position.set(x, y);

        return bodyDef;
    }

    @Override
    protected FixtureDef getFixtureDefinition() {
        return null;
    }

    @Override
    protected int getGameObjectLayer() {
        return 0;
    }

    @Override
    protected void onSizeChange(float oldSize) {

    }

    @Override
    protected void update(float dt) {

    }

    protected void  setFixtures() {
        while (getBody().getFixtureList().size > 0) {
            getBody().destroyFixture(getBody().getFixtureList().get(0));
        }

        //Left
        FixtureDef fixture1 = new FixtureDef();
        PolygonShape leftBox = new PolygonShape();
        leftBox.setAsBox(0.5f, ySize / 2, new Vector2(-xSize / 2 - 0.5f, 0), 0);
        fixture1.shape = leftBox;

        //Up
        FixtureDef fixture2 = new FixtureDef();
        PolygonShape upperBox = new PolygonShape();
        upperBox.setAsBox(xSize / 2, 0.5f, new Vector2(0, ySize / 2 + 0.5f), 0);
        fixture2.shape = upperBox;

        //Right
        FixtureDef fixture3 = new FixtureDef();
        PolygonShape rightBox = new PolygonShape();
        rightBox.setAsBox(0.5f, ySize / 2, new Vector2(xSize / 2 + 0.5f, 0), 0);
        fixture3.shape = rightBox;

        //Lower
        FixtureDef fixture4 = new FixtureDef();
        PolygonShape lowerBox = new PolygonShape();
        lowerBox.setAsBox(xSize / 2, 0.5f, new Vector2(0, -ySize / 2 - 0.5f), 0);
        fixture4.shape = lowerBox;

        getBody().createFixture(fixture1);
        getBody().createFixture(fixture2);
        getBody().createFixture(fixture3);
        getBody().createFixture(fixture4);

        leftBox.dispose();
        upperBox.dispose();
        rightBox.dispose();
        lowerBox.dispose();
    }
}
