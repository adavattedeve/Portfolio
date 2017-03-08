package com.mygdx.game;

import com.badlogic.gdx.scenes.scene2d.Actor;
import com.mygdx.mygameutilities.MyActor;
import com.mygdx.mygameutilities.State;

/**
 * Created by atter on 04-Mar-17.
 */

public class LifeIconActor extends MyActor {

    public LifeIconActor(State state) {
        super(state);
        setTex(((PhysicsGame)state.getGame()).playerTex);
        setSize(25f);
        setBounds(0, 0, getSizeX(), getSizeY());
        setRotation(45f);
    }
}
