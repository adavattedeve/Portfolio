package com.mygdx.game;

import com.badlogic.gdx.Gdx;
import com.badlogic.gdx.Input;
import com.badlogic.gdx.graphics.g2d.SpriteBatch;
import com.mygdx.mygameutilities.GameStateManager;
import com.mygdx.mygameutilities.MyGame;
import com.mygdx.mygameutilities.State;

/**
 * Created by atter on 30-Jan-17.
 */

public class PauseState extends State {

    public PauseState(GameStateManager gsm, MyGame game) {
        super(gsm, game);
    }

    @Override
    protected void onStart() {

    }

    @Override
    protected void handleInput() {
        if (Gdx.input.isKeyJustPressed(Input.Keys.SPACE) || Gdx.input.justTouched()) {
            gsm.pop();
        }
    }

    @Override
    protected void update(float dt) {
    }

    @Override
    public void dispose() {

    }

    @Override
    public void render(SpriteBatch batch) {

    }
}
