package com.mygdx.mygameutilities;

import com.badlogic.gdx.ApplicationAdapter;
import com.badlogic.gdx.Game;
import com.badlogic.gdx.Gdx;
import com.badlogic.gdx.Screen;
import com.badlogic.gdx.graphics.g2d.SpriteBatch;
import com.badlogic.gdx.math.MathUtils;
import com.badlogic.gdx.math.Vector2;

/**
 * Created by atter on 28-Feb-17.
 */

public abstract class MyGame extends Game {

    protected static GameStateManager gsm;
    protected static CameraShaker camShaker;

    private SpriteBatch batch;
    private static float timeScale = 1;

    @Override
    public final void create () {
        batch = new SpriteBatch();
        gsm = new GameStateManager(this);
        camShaker = new CameraShaker();
        onCreate();
    }

    @Override
    public final void render () {
        float dt = Gdx.graphics.getDeltaTime();
        TimeScaleManipulator.update(dt);
        dt *= timeScale;
        gsm.update(dt);
        camShaker.update(dt);
        gsm.render(batch);
        super.render();
    }

    @Override
    public void dispose () {
        gsm.dispose();
        batch.dispose();
        onDispose();
    }

    @Override
    public void setScreen(Screen screen) {
        super.setScreen(screen);
        Gdx.input.setInputProcessor(((State)screen).getStage());
    }

    public static void shakeCamera(float power, float time) {
        camShaker.Shake(gsm.peek().getCamera(), power, time);
    }
    public static void addGameObject(GameObject go, int layerIndex) {
        gsm.peek().addGameObject(go, layerIndex);
    }

    public static void removeGameObject(GameObject go, int layerIndex) {
        gsm.peek().removeGameObject(go, layerIndex);
    }

    public static float getTimeScale() {
        return timeScale;
    }

    public static void setTimeScale(float timeScale) {
        if (timeScale > 0) {
            MyGame.timeScale = timeScale;
        }
    }

    public SpriteBatch getBatch() {
        return batch;
    }

    public abstract void onCreate();
    public abstract Vector2 getViewportSize();
    public abstract Vector2 getGuiViewportSize();
    protected abstract void onDispose();
}
