package com.mygdx.game;

import com.badlogic.gdx.Gdx;
import com.badlogic.gdx.Input;
import com.badlogic.gdx.math.Interpolation;
import com.badlogic.gdx.math.Vector2;
import com.badlogic.gdx.physics.box2d.ContactListener;
import com.badlogic.gdx.scenes.scene2d.actions.MoveByAction;
import com.badlogic.gdx.scenes.scene2d.actions.MoveToAction;
import com.badlogic.gdx.scenes.scene2d.actions.RunnableAction;
import com.badlogic.gdx.scenes.scene2d.actions.SequenceAction;
import com.badlogic.gdx.scenes.scene2d.ui.Table;
import com.badlogic.gdx.scenes.scene2d.ui.TextButton;
import com.mygdx.mygameutilities.GameStateManager;
import com.mygdx.mygameutilities.MyGame;
import com.mygdx.mygameutilities.PhysicsParameters;
import com.mygdx.mygameutilities.PhysicsState;
import com.mygdx.mygameutilities.TextObject;

import java.util.ArrayList;

/**
 * Created by atter on 29-Jan-17.
 */

public class PlayState extends PhysicsState {

    private MeteoriteSpawner meteoriteSpawner;
    private SpaceShip player;
    private TextObject scoreText;

    private TextButton rotateLeftButton;
    private TextButton rotateRightButton;
    private TextButton accerelateButton;
    private TextButton shootButton;

    private ArrayList<LifeIconActor> lifeIcons;
    private Table lifeTable;

    private int score = 0;
    private float timeSurvived;
    private float gameOverTimer;

    private float gameOverDelay = 3.0f;

    public PlayState(GameStateManager gsm, MyGame game) {
        super(gsm, game, new PhysicsParameters(
                PhysicsGame.TIME_STEP,
                PhysicsGame.MAX_FRAME_TIME,
                new Vector2(0, PhysicsGame.GRAVITY),
                PhysicsGame.VELOCITY_ITERATIONS,
                PhysicsGame.POSITION_ITERATIONS
        ));
        debugPhysics = PhysicsGame.DEBUG_PHYSICS;
    }

    public void addScores(int score) {
        this.score += score;
    }

    @Override
    protected void onStart() {

        gameOverTimer = 0.0f;
        timeSurvived = 0.0f;

        scoreText = new TextObject(((PhysicsGame)getGame()).font, this);
        scoreText.setPosition(0.05f, cam.viewportHeight - 0.05f);

        meteoriteSpawner = new MeteoriteSpawner(this);

        player = new SpaceShip(PhysicsGame.WORLD_WIDTH / 2, PhysicsGame.WORLD_HEIGHT / 2, this);
        new Boundaries(PhysicsGame.WORLD_WIDTH / 2,
                PhysicsGame.WORLD_START_Y + PhysicsGame.WORLD_HEIGHT / 2,
                PhysicsGame.WORLD_WIDTH, PhysicsGame.WORLD_HEIGHT, this);

        new Background(this);
        createButtons();

        //Life icons
        lifeIcons = new ArrayList<LifeIconActor>();
        lifeTable = new Table();
        lifeTable.setFillParent(true);
        lifeTable.top();
        lifeTable.defaults().space(5);
        for (int i = 0; i < player.getHealth(); ++i) {
            lifeIcons.add(new LifeIconActor(this));
            lifeTable.add(lifeIcons.get(i));
        }

        stage.addActor(lifeTable);
    }

    @Override
    protected void handleInput() {
        if (player.isDestroyed()) {
            return;
        }
        player.setAccelerate(Gdx.input.isKeyPressed(Input.Keys.DPAD_UP) ||
                accerelateButton.isPressed());
        if (Gdx.input.isKeyPressed(Input.Keys.DPAD_RIGHT) ||
                rotateRightButton.isPressed()) {
            player.setRotationDirection(-1);
        } else if (Gdx.input.isKeyPressed(Input.Keys.DPAD_LEFT)  ||
                rotateLeftButton.isPressed()) {
            player.setRotationDirection(1);
        } else {
            player.setRotationDirection(0);
        }

        if (Gdx.input.isKeyPressed(Input.Keys.SPACE) ||shootButton.isPressed()) {
            player.shoot();
        }

        if (Gdx.input.isKeyJustPressed(Input.Keys.T) ) {
            MyGame.setTimeScale(0.5f);
        }
        if (Gdx.input.isKeyJustPressed(Input.Keys.Y) ) {
            MyGame.setTimeScale(1.5f);
        }

    }

    @Override
    protected void update(float dt) {

        if (lifeIcons.size() != player.getHealth()) {
            updatePlayerLifeIcons();
        }

        meteoriteSpawner.update(dt);

        scoreText.setText("Score: " + score + "\n" +
        "Time: " + (int)timeSurvived);

        if (player.isDestroyed()) {
            gameOverTimer += dt;
        } else {
            timeSurvived += dt;
        }
        if (gameOverTimer > gameOverDelay) {
            ((PhysicsGame)getGame()).gameOver(score, timeSurvived);
        }
    }

    @Override
    public void dispose() {
    }

    @Override
    protected ContactListener getContactListener() {
        return new MyContactListener();
    }

    private void updatePlayerLifeIcons() {
        if (lifeIcons.isEmpty())
            return;

        while (player.getHealth() < lifeIcons.size()) {
            final LifeIconActor lifeIcon = lifeIcons.get(lifeIcons.size() - 1);
            lifeIcons.remove(lifeIcon);

            MoveByAction action = new MoveByAction();
            action.setAmountY(50f);
            action.setDuration(0.75f);
            RunnableAction runnable = new RunnableAction();
            runnable.setRunnable(new Runnable() {
                @Override
                public void run() {
                    lifeTable.removeActor(lifeIcon);
                }
            });

            SequenceAction seq = new SequenceAction();
            seq.addAction(action);
            seq.addAction(runnable);

            lifeIcon.addAction(seq);
        }
    }

    private void createButtons() {

        float buttonSize = 100;
        float betweenButtons = 5f;
        float buttonY = 10f;
        float buttonAspectRatio = 1.5f;

        //Initialize rotate left button
        rotateLeftButton = new TextButton("Left",
                ((PhysicsGame)getGame()).skin);

        //Initialize rotate right button
        rotateRightButton = new TextButton("Right",
                ((PhysicsGame)getGame()).skin);

        //Initialize accerelate button
        accerelateButton = new TextButton("Accerelate",
                ((PhysicsGame)getGame()).skin);


        //Initialize shoot button
        shootButton = new TextButton("Shoot",
                ((PhysicsGame)getGame()).skin);

        shootButton.setWidth(buttonSize * buttonAspectRatio);
        shootButton.setHeight(buttonSize);
        shootButton.setPosition(accerelateButton.getX() - betweenButtons - shootButton.getWidth(),
                buttonY);

        Table inputTableLeft = new Table();
        inputTableLeft.defaults().width(buttonSize * buttonAspectRatio)
                .height(buttonSize).pad(betweenButtons);
        inputTableLeft.add(rotateLeftButton);
        inputTableLeft.add(rotateRightButton);

        Table inputTableRight = new Table();
        inputTableRight.defaults().width(buttonSize * buttonAspectRatio)
                .height(buttonSize).pad(betweenButtons);
        inputTableRight.add(shootButton);
        inputTableRight.add(accerelateButton);

        Table inputTable = new Table();
        inputTable.setFillParent(true);
        inputTable.bottom();
        inputTable.add(inputTableLeft).expandX().left();
        inputTable.add(inputTableRight).expandX().right();
        stage.addActor(inputTable);


        //Fade in action
        float yMovement = 100f;
        MoveByAction first = new MoveByAction();
        first.setAmountY(-yMovement);
        first.setDuration(0);

        MoveByAction second = new MoveByAction();
        second.setAmountY(yMovement);
        second.setDuration(1.25f);
        second.setInterpolation(Interpolation.elasticOut);

        SequenceAction sequence = new SequenceAction();
        sequence.addAction(first);
        sequence.addAction(second);

        inputTable.addAction(sequence);
    }
}
