package com.mygdx.mygameutilities;

import com.badlogic.gdx.graphics.Texture;
import com.badlogic.gdx.graphics.g2d.SpriteBatch;
import com.badlogic.gdx.graphics.g2d.TextureRegion;
import com.badlogic.gdx.scenes.scene2d.Actor;

/**
 * Created by atter on 29-Jan-17.
 */

public abstract class GameObject{

    protected TextureRegion textureRegion;
    protected int layer = 0;

    private State state;
    private Texture tex;
    private float aspectRatio = 1.0f;
    private float rotation = 0.0f;
    private float size = 1.0f;
    private float x = 0.0f;
    private float y = 0.0f;

    public GameObject(State state) {
        this.state = state;
        layer = getGameObjectLayer();
        MyGame.addGameObject(this, layer);
    }

    public Texture getTex() {
        return tex;
    }

    public float getSize() {
        return size;
    }

    public float getSizeX() {
        return size * aspectRatio;
    }

    public float getSizeY() {
        return size;
    }

    public float getRotation() {
        return rotation;
    }

    public float getAspectRatio() {
        return aspectRatio;
    }

    public float getX() {
        return x;
    }

    public float getY() {
        return y;
    }

    public void setRotation(float rotation) {
        this.rotation = rotation;
        if (this.rotation >= 360) {
            this.rotation -= 360;
        } else if (this.rotation < 0) {
            this.rotation += 360;
        }
    }

    public State getState() {
        return state;
    }

    public void setTex(Texture tex) {
        this.tex = tex;
        setTextureRegion(new TextureRegion(tex));
    }

    public void setTextureRegion(TextureRegion texReg) {
        setAspectRatio((float)(texReg.getRegionWidth()) / texReg.getRegionHeight());
        textureRegion = texReg;
    }

    public void setSize(float size) {
        if (size <= 0) {
            throw new IllegalArgumentException();
        }
        float oldSize = this.size;
        this.size = size;
        onSizeChange(oldSize);
    }

    public void setAspectRatio(float aspectRatio) {
        if (aspectRatio <= 0) {
            throw new IllegalArgumentException();
        }
        if (this.aspectRatio != aspectRatio) {
            this.aspectRatio = aspectRatio;
            onSizeChange(size);
        }
    }

    public void setX(float x) {
        this.x = x;
    }

    public void setY(float y) {
        this.y = y;
    }

    public void setPosition(float x, float y) {
        setX(x);
        setY(y);
    }

    public void dispose() {
        MyGame.removeGameObject(this, layer);
    }

    public void render(SpriteBatch batch) {
        batch.draw(textureRegion, getX(), getY(),
                getAspectRatio() * getSize() / 2,
                getSize() / 2,
                getAspectRatio() * getSize(), getSize(), 1, 1,
                getRotation());
    }

    protected abstract int getGameObjectLayer();

    protected abstract void onSizeChange(float oldSize);
    protected abstract void update(float dt);
}
