package com.mygdx.mygameutilities;

import com.badlogic.gdx.graphics.g2d.Animation;
import com.badlogic.gdx.graphics.g2d.SpriteBatch;
import com.badlogic.gdx.graphics.g2d.TextureRegion;

/**
 * Created by atter on 07-Mar-17.
 */

public abstract class AnimatedGameObject extends GameObject {

    private Animation<TextureRegion> currentAnimation;
    protected float stateTime = 0;

    public AnimatedGameObject(State state) {
        super(state);
    }


    @Override
    protected void onSizeChange(float oldSize) {

    }

    @Override
    protected void update(float dt) {
        setTextureRegion(currentAnimation.getKeyFrame(stateTime, true));
        stateTime += dt;
    }

    public void setCurrentAnimation(Animation<TextureRegion> currentAnimation) {
        this.currentAnimation = currentAnimation;
        stateTime = 0;
        setTextureRegion(currentAnimation.getKeyFrame(stateTime));
    }

    public Animation<TextureRegion> getCurrentAnimation() {
        return currentAnimation;
    }
}
