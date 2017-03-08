package com.mygdx.game;

import com.badlogic.gdx.graphics.g2d.Animation;
import com.badlogic.gdx.graphics.g2d.TextureRegion;
import com.mygdx.mygameutilities.AnimatedGameObject;
import com.mygdx.mygameutilities.State;

/**
 * Created by atter on 07-Mar-17.
 */

public class SmokeEffect  extends AnimatedGameObject {

    public SmokeEffect(State state, float x, float y) {
        super(state);
        setSize(0.5f);
        setCurrentAnimation(new Animation<TextureRegion>(1 / 60f,
                ((PhysicsGame)state.getGame()).smokeAnimTextures));
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
