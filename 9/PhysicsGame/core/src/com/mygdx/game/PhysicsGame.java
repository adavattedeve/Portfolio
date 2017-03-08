package com.mygdx.game;

import com.badlogic.gdx.Gdx;
import com.badlogic.gdx.audio.Music;
import com.badlogic.gdx.audio.Sound;
import com.badlogic.gdx.graphics.Color;
import com.badlogic.gdx.graphics.Texture;
import com.badlogic.gdx.graphics.g2d.BitmapFont;
import com.badlogic.gdx.graphics.g2d.TextureRegion;
import com.badlogic.gdx.graphics.g2d.freetype.FreeTypeFontGenerator;
import com.badlogic.gdx.math.Vector2;
import com.badlogic.gdx.scenes.scene2d.ui.Skin;
import com.mygdx.mygameutilities.MyGame;
import com.mygdx.mygameutilities.MyUtilities;

public class PhysicsGame extends MyGame {
	public static final float VIEWPORT_WIDTH = 8f;
	public static final float VIEWPORT_HEIGHT = 5f;
    public static final float GUI_VIEWPORT_WIDTH = 800f;
    public static final float GUI_VIEWPORT_HEIGHT = 500f;
    public static final float WORLD_WIDTH = 8f;
    public static final float WORLD_HEIGHT = 4f;
    public static final float WORLD_START_Y = 1f;

    public final static boolean DEBUG_PHYSICS = false;
    public final static float TIME_STEP = 1 / 60f;
    public final static float MAX_FRAME_TIME = 1 / 4f;
    public final static float GRAVITY = 0f;
    public final static int VELOCITY_ITERATIONS = 8;
    public final static int POSITION_ITERATIONS = 3;

    public Texture backgroundTex;
    public Texture laserGreenTex;
    public Texture meteorBigTex;
    public Texture meteorSmallTex;
    public Texture playerTex;

    private Texture explosionSheet;
    private Texture smokeSheet;
    public TextureRegion[] explosionAnimTextures;
    public TextureRegion[] smokeAnimTextures;

    public Music backgroundMusic;

    public Sound meteorWallSound;
    public Sound meteorDamageSound;
    public Sound meteorMeteorSound;
    public Sound meteorDestructionSound;

    public Sound spacehipAccerelateSound;
    public Sound spaceshipShootSound;
    public Sound spaceshipDestructionSound;

    public BitmapFont font;
    public Skin skin;

    private int highScore = 0;
    private float longestTimeSurvived = 0;

	@Override
	public void onCreate () {
        loadAssets();
        backgroundMusic.play();
        backgroundMusic.setLooping(true);
        backgroundMusic.setVolume(0.4f);
        gsm.push(new MenuState(gsm, this));
	}

    @Override
    public void pause() {
        gsm.push(new PauseState(gsm, this));
    }

    @Override
    public Vector2 getViewportSize() {
        return new Vector2(VIEWPORT_WIDTH, VIEWPORT_HEIGHT);
    }

    @Override
    public Vector2 getGuiViewportSize() {
        return new Vector2(GUI_VIEWPORT_WIDTH, GUI_VIEWPORT_HEIGHT);
    }

    @Override
    public void onDispose() {
        backgroundTex.dispose();
        laserGreenTex.dispose();
        meteorBigTex.dispose();
        meteorSmallTex.dispose();
        playerTex.dispose();

        backgroundMusic.dispose();

        meteorWallSound.dispose();
        meteorDamageSound.dispose();
        meteorMeteorSound.dispose();
        meteorDestructionSound.dispose();

        spacehipAccerelateSound.dispose();
        spaceshipShootSound.dispose();
        spaceshipDestructionSound.dispose();

        font.dispose();
        skin.dispose();
    }

    public void gameOver(int score, float timeSurvived) {
        if (score > highScore) {
            highScore = score;
        }

        if (timeSurvived > longestTimeSurvived) {
            longestTimeSurvived = timeSurvived;
        }
        gsm.set(new GameOverState(score, timeSurvived, gsm,this));
    }

    public int getHighScore() {
        return highScore;
    }

    public float getLongestTimeSurvived() {
        return longestTimeSurvived;
    }

    private void loadAssets() {

        backgroundTex = new Texture("background.png");
        laserGreenTex = new Texture("laserGreen.png");
        meteorBigTex = new Texture("meteorBig.png");
        meteorSmallTex = new Texture("meteorSmall.png");
        playerTex = new Texture("player.png");
        explosionSheet = new Texture("ExplosionSpriteSheet.png");
        smokeSheet = new Texture("SmokeSpriteSheet.png");

        backgroundMusic = Gdx.audio.newMusic(Gdx.files.internal("BackgroundMusic.mp3"));

        meteorWallSound = Gdx.audio.newSound(Gdx.files.internal("MeteorWallCollision.wav"));
        meteorDamageSound = Gdx.audio.newSound(Gdx.files.internal("MeteorTakeDamage.wav"));
        meteorMeteorSound =
                Gdx.audio.newSound(Gdx.files.internal("MeteorMeteorCollision.wav"));
        meteorDestructionSound = Gdx.audio.newSound(Gdx.files.internal("MeteorDestruction.wav"));

        spacehipAccerelateSound = Gdx.audio.newSound(Gdx.files.internal("SpaceshipAccelerate.wav"));
        spaceshipShootSound = Gdx.audio.newSound(Gdx.files.internal("SpaceshipShoot.wav"));
        spaceshipDestructionSound =
                Gdx.audio.newSound(Gdx.files.internal("SpaceshipDestruction.wav"));

        FreeTypeFontGenerator fontGen = new FreeTypeFontGenerator(Gdx.files.internal("font.TTF"));

        FreeTypeFontGenerator.FreeTypeFontParameter parameter =
                new FreeTypeFontGenerator.FreeTypeFontParameter();
        parameter.size = 28;
        parameter.borderColor = Color.FOREST;
        font = fontGen.generateFont(parameter);

        skin = new Skin(Gdx.files.internal("uiskin.json"));

        TextureRegion[][] tmp = TextureRegion.split(
                explosionSheet, explosionSheet.getWidth() / 10,
                explosionSheet.getHeight() / 5);

        explosionAnimTextures = MyUtilities.transformTo1D(tmp, 10, 5, 49);

        tmp = TextureRegion.split(
                smokeSheet, smokeSheet.getWidth() / 20,
                smokeSheet.getHeight() / 1);
        smokeAnimTextures = MyUtilities.transformTo1D(tmp, 20, 1, 10);
    }
}
