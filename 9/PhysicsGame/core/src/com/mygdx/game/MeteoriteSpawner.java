package com.mygdx.game;

import com.mygdx.mygameutilities.PhysicsState;

/**
 * Created by atter on 30-Jan-17.
 */

public class MeteoriteSpawner {
    private PhysicsState state;

    private float meteorTickTimer;

    private float meteorTickTime;
    private float chancePerTick;
    private float meteorSmallWeight;
    private float meteorMediumWeight;
    private float meteorBigWeight;

    private int missedTicksInRow = 0;

    public MeteoriteSpawner(PhysicsState state) {
        this.state = state;
        meteorTickTime = 2.0f;
        chancePerTick = 0.25f;

        meteorSmallWeight = 1.0f;
        meteorMediumWeight = 0.5f;
        meteorBigWeight = 0.1f;
        meteorTickTimer = 0.0f;
        spawnMeteorite();
    }

    public void update(float dt) {
        meteorTickTime -= dt / 120;
        chancePerTick += dt / 600;
        meteorSmallWeight += dt / 1800;
        meteorMediumWeight += dt / 600;
        meteorBigWeight += dt / 300;

        meteorTickTimer += dt;
        if (meteorTickTimer >= meteorTickTime) {
            meteorTickTimer -= meteorTickTime;
            if ((float)Math.random() < chancePerTick || missedTicksInRow >= 3) {
                missedTicksInRow = 0;
                spawnMeteorite();
            } else {
                missedTicksInRow++;
            }
        }
    }


    private void spawnMeteorite() {
        float totalWeight = meteorSmallWeight + meteorMediumWeight + meteorBigWeight;
        float random = (float)Math.random() * totalWeight;
        if (random < meteorSmallWeight) {
            new MeteoriteSmall(state);
        } else if (random < meteorMediumWeight + meteorSmallWeight) {
            new MeteoriteMedium(state);
        } else {
            new MeteoriteBig(state);
        }
    }

}
