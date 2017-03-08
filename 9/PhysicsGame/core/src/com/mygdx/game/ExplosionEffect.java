package com.mygdx.game;

import com.badlogic.gdx.graphics.g2d.Animation;
import com.badlogic.gdx.graphics.g2d.SpriteBatch;
import com.badlogic.gdx.graphics.g2d.TextureRegion;
import com.mygdx.mygameutilities.AnimatedGameObject;
import com.mygdx.mygameutilities.State;

/**
 * Created by atter on 07-Mar-17.
 */

public class ExplosionEffect extends AnimatedGameObject {

    public ExplosionEffect(State state, float x, float y) {
        super(state);
        setCurrentAnimation(new Animation<TextureRegion>(1 / 60f,
                ((PhysicsGame)state.getGame()).explosionAnimTextures));
        setPosition(x, y);
    }

    @Override
    public void update(float dt) {
        if (getCurrentAnimation().isAnimationFinished(stateTime))
            dispose();

        super.update(dt);
    }

    @Override
    protected int getGameObjectLayer() {
        return 2;
    }
}
