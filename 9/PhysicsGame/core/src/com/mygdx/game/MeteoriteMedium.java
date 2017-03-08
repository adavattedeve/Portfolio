package com.mygdx.game;

import com.badlogic.gdx.graphics.Texture;
import com.mygdx.mygameutilities.*;

/**
 * Created by atter on 30-Jan-17.
 */

public class MeteoriteMedium extends Meteorite {

    private int splitCount = 2;
    public MeteoriteMedium(PhysicsState state) {
        super(state);
    }
    public MeteoriteMedium(float x, float y, PhysicsState state) {
        super(x, y, state);
    }

    @Override
    protected void initializeValues() {
        setTex(((PhysicsGame)getState().getGame()).meteorSmallTex);
        setSize(0.6f);
        movementForce = 10;
        maxHealth = 3;
        minSpeed = 1.5f;
        scoreValue = 2;
        density = 200f;
    }

    @Override
    protected void splitIntoSmallerMeteorites() {
        MyGame.shakeCamera(0.15f, 0.1f);
        for (int i = 0; i < splitCount; ++i) {
            new MeteoriteSmall(getX(), getY(), ((PhysicsState)getState()));
        }
    }
}
