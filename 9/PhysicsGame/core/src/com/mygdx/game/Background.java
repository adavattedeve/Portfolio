package com.mygdx.game;

import com.badlogic.gdx.graphics.Texture;
import com.mygdx.mygameutilities.State;
import com.mygdx.mygameutilities.GameObject;
/**
 * Created by atter on 30-Jan-17.
 */

public class Background extends GameObject {
    private final float SCALE = 1.5f;
    public Background(State state) {
        super(state);
        setTex(((PhysicsGame)state.getGame()).backgroundTex);
        float w = PhysicsGame.VIEWPORT_WIDTH * SCALE;
        float h = PhysicsGame.VIEWPORT_HEIGHT * SCALE;
        setX(-(w - PhysicsGame.VIEWPORT_WIDTH) / 2);
        setY(-(h - PhysicsGame.VIEWPORT_HEIGHT) / 2 + 0.5f);
        setSize(w/getAspectRatio());
    }

    @Override
    protected void onSizeChange(float oldSize) {

    }

    @Override
    public void update(float dt) {

    }

    @Override
    protected int getGameObjectLayer() {
        return -5;
    }
}
