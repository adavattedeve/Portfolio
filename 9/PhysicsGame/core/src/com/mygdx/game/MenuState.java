package com.mygdx.game;

import com.badlogic.gdx.Gdx;
import com.badlogic.gdx.Input;
import com.badlogic.gdx.scenes.scene2d.ui.TextButton;
import com.mygdx.mygameutilities.GameStateManager;
import com.mygdx.mygameutilities.MyGame;
import com.mygdx.mygameutilities.State;
import com.mygdx.mygameutilities.TextObject;

/**
 * Created by atter on 29-Jan-17.
 */

public class MenuState extends State {

    public MenuState(GameStateManager gsm, MyGame game) {
        super(gsm, game);
    }

    @Override
    protected void onStart() {
        new Background(this);
        TextObject temp = new TextObject(((PhysicsGame)getGame()).font, this);
        temp.setText("Tap to play!");
        temp.setPosition(PhysicsGame.VIEWPORT_WIDTH / 2 - temp.getWidth() / 2,
                2 * cam.viewportHeight / 3);

        TextButton rotateLeftButton = new TextButton("Left",
                ((PhysicsGame)getGame()).skin);
    }

    @Override
    protected void handleInput() {
        if (Gdx.input.isKeyPressed(Input.Keys.SPACE) || Gdx.input.justTouched()) {
            gsm.set(new PlayState(gsm, getGame()));
        }
    }

    @Override
    protected void update(float dt) {
    }

    @Override
    public void dispose() {

    }

}
