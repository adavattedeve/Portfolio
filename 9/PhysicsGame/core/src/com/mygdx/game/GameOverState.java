package com.mygdx.game;

import com.mygdx.mygameutilities.MyGame;
import com.mygdx.mygameutilities.GameStateManager;
import com.mygdx.mygameutilities.TextObject;
/**
 * Created by atter on 29-Jan-17.
 */


public class GameOverState extends MenuState {

    private int score;
    private int timeSurvived;
    public GameOverState(int score, float timeSurvived, GameStateManager gsm, MyGame game) {
        super(gsm, game);
        this.score = score;
        this.timeSurvived = (int)timeSurvived;
    }

    @Override
    protected void onStart() {
        super.onStart();
        String scores = "Score: " + score + "          Highscore: " +
                ((PhysicsGame)getGame()).getHighScore();
        scores += "\nTime: " + (int)timeSurvived + "         Longest time: " +
                (int)((PhysicsGame)getGame()).getLongestTimeSurvived();

        TextObject text = new TextObject(((PhysicsGame)getGame()).font, this);

        text.setText(scores);
        text.setPosition(PhysicsGame.VIEWPORT_WIDTH / 2 - text.getWidth() / 2,
                PhysicsGame.VIEWPORT_HEIGHT / 2);
    }
}
