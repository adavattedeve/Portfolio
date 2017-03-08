package com.mygdx.game;

import com.badlogic.gdx.graphics.Texture;
import com.badlogic.gdx.physics.box2d.World;
import com.mygdx.mygameutilities.MyGame;
import com.mygdx.mygameutilities.PhysicsState;

/**
 * Created by atter on 30-Jan-17.
 */

public class MeteoriteSmall extends  Meteorite {
    public MeteoriteSmall(PhysicsState state) {
        super(state);
    }
    public MeteoriteSmall(float x, float y, PhysicsState state) {
        super(x, y, state);
    }

    @Override
    protected void initializeValues() {
        setTex(((PhysicsGame)getState().getGame()).meteorSmallTex);
        setSize(0.3f);
        movementForce = 5;
        maxHealth = 1;
        minSpeed = 2f;
    }

    @Override
    protected void splitIntoSmallerMeteorites() {
        MyGame.shakeCamera(0.05f, 0.1f);
    }
}
