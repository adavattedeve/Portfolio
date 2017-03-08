package com.mygdx.game;

import com.badlogic.gdx.graphics.Texture;
import com.mygdx.mygameutilities.*;

/**
 * Created by atter on 30-Jan-17.
 */

public class MeteoriteBig extends Meteorite {

    private int splitCount = 2;

    public MeteoriteBig(PhysicsState state) {
        super(state);
    }
    public MeteoriteBig(float x, float y, PhysicsState state) {
        super(x, y, state);
    }

    @Override
    protected void initializeValues() {
        setTex(((PhysicsGame)getState().getGame()).meteorBigTex);
        setSize(1f);
        movementForce = 15;
        maxHealth = 5;
        minSpeed = 0.6f;
        scoreValue = 5;
        density = 400f;
    }

    @Override
    protected void splitIntoSmallerMeteorites() {
        MyGame.shakeCamera(0.2f, 0.1f);
        for (int i = 0; i < splitCount; ++i) {
            new MeteoriteMedium(getX(), getY(), ((PhysicsState)getState()));
        }
    }
}
