package com.mygdx.mygameutilities;

import com.badlogic.gdx.math.MathUtils;

/**
 * Created by atter on 03-Mar-17.
 */

public class TimeScaleManipulator {

    private static float defaultTimeScale = 1;

    private static float lerpSpeed = 1;
    private static float duration = 1;
    private static float timer = 0;
    // start, holdTime
    public static void slowMotion(float startScale, float duration, float lerpSpeed) {
        if (startScale < 0 || lerpSpeed < 0)
            return;
        timer = 0;
        TimeScaleManipulator.duration = duration;
        TimeScaleManipulator.lerpSpeed = lerpSpeed;
        MyGame.setTimeScale(startScale);
    }

    public static void update(float dt) {
        timer += dt;
        if (timer > duration) {
            MyGame.setTimeScale (MathUtils.lerp(MyGame.getTimeScale(), defaultTimeScale,
                            (MyGame.getTimeScale() / defaultTimeScale) * lerpSpeed * dt));
        }
    }

    public static void setDefaultTimeScale(float timeScale) {
        defaultTimeScale = timeScale;
    }
}
