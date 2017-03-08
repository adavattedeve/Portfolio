package com.mygdx.mygameutilities;

import com.badlogic.gdx.graphics.OrthographicCamera;

/**
 * Created by atter on 30-Jan-17.
 */

public class CameraShaker {

    private OrthographicCamera cam;
    private float time = 0;
    private float originalX, originalY;
    private float currentTime = 0;
    private float power = 0;
    private float currentPower = 0;
    private boolean shaking = false;


    public void Shake(OrthographicCamera cam, float power, float time) {
        if (this.cam != cam ) {
            if (this.cam != null) {
                cam.position.x = originalX;
                cam.position.y = originalY;
            }

            this.cam = cam;
            originalX = cam.position.x;
            originalY = cam.position.y;
        }

        this.power = power;
        this.time = time;
        currentTime = 0;
        shaking = true;
    }

    public void update(float dt){
        if (!shaking)
            return;

        if(currentTime < time) {
            currentPower = power * ((time - currentTime) / time);

            float x = ((float)Math.random() - 0.5f) * 2 * currentPower;
            float y = ((float)Math.random() - 0.5f) * 2 * currentPower;

            cam.position.x += x;
            cam.position.y += y;
            currentTime += dt;
        } else {
            cam.position.x = originalX;
            cam.position.y = originalY;
            shaking = false;
        }
    }

}

