package com.mygdx.mygameutilities;

import com.badlogic.gdx.Gdx;
import com.badlogic.gdx.Screen;
import com.badlogic.gdx.graphics.GL20;
import com.badlogic.gdx.graphics.OrthographicCamera;
import com.badlogic.gdx.graphics.g2d.SpriteBatch;
import com.badlogic.gdx.scenes.scene2d.Stage;
import com.badlogic.gdx.scenes.scene2d.ui.Table;
import com.badlogic.gdx.utils.viewport.FitViewport;
import com.mygdx.game.SpaceShip;

import java.util.ArrayList;
import java.util.HashMap;
import java.util.Set;

/**
 * Created by atter on 28-Jan-17.
 */

public abstract class State implements Screen {
    private MyGame game;
    private HashMap<Integer, ArrayList<GameObject>> gameObjects;
    private ArrayList<Integer> keysInOrder;
    private ArrayList<TextObject> textObjects;
    protected GameStateManager gsm;

    protected OrthographicCamera cam;
    protected OrthographicCamera guiCam;

    protected float guiCamToCamW;
    protected float guiCamToCamH;

    protected Stage stage;

    public State(GameStateManager gsm, MyGame game) {
        this.gsm = gsm;
        this.game = game;
        cam = new OrthographicCamera();
        cam.setToOrtho(false, game.getViewportSize().x, game.getViewportSize().y);
        guiCam = new OrthographicCamera();
        guiCam.setToOrtho(false, game.getGuiViewportSize().x, game.getGuiViewportSize().y);
        gameObjects = new HashMap<Integer, ArrayList<GameObject>>();
        keysInOrder = new ArrayList<Integer>();
        textObjects = new ArrayList<TextObject>();
        stage = new Stage(new FitViewport(guiCam.viewportWidth, guiCam.viewportHeight, guiCam),
                getGame().getBatch());
        guiCamToCamW = guiCam.viewportWidth/cam.viewportWidth;
        guiCamToCamH = guiCam.viewportHeight/cam.viewportHeight;
    }

    public void addGameObject(GameObject go, int layer) {
        if (!gameObjects.containsKey(layer)) {
            gameObjects.put(layer, new ArrayList<GameObject>());
            updateKeysInOrderList();
        }
        gameObjects.get(layer).add(go);
    }

    public void removeGameObject(GameObject go, int layer) {
        if (gameObjects.containsKey(layer)) {
            gameObjects.get(layer).remove(go);
        }
    }

    public void addTextObject(TextObject tO) {
        textObjects.add(tO);
    }

    public void removeTextObject(TextObject tO) {
        textObjects.remove(tO);
    }

    public OrthographicCamera getCamera() {
        return cam;
    }

    public void updateState(float dt) {
        handleInput();
        updateGameObjects(dt);
        update(dt);
        stage.act(dt);
    }

    public void render(SpriteBatch batch) {
        Gdx.gl.glClear(GL20.GL_COLOR_BUFFER_BIT);

        cam.update();
        guiCam.update();
        batch.setProjectionMatrix(cam.combined);
        batch.begin();
        renderGameObjects(batch);
        batch.setProjectionMatrix(guiCam.combined);
        renderTextObjects(batch);
        batch.end();

        stage.draw();
    }

    public void disposeState() {
        for (int keyIndex = 0; keyIndex < keysInOrder.size(); ++keyIndex) {
            ArrayList<GameObject> goList = gameObjects.get(keysInOrder.get(keyIndex));
            while(!goList.isEmpty()) {
                goList.get(0).dispose();
            }
        }

        while(!textObjects.isEmpty()) {
            textObjects.get(0).dispose();
        }
        dispose();
    }

    public MyGame getGame() {
        return game;
    }

    public Stage getStage() {
        return stage;
    }

    public float getGuiCamToCamW() {
        return guiCamToCamW;
    }

    public float getGuiCamToCamH() {
        return guiCamToCamH;
    }

    @Override
    public void render(float dt) {

    }

    @Override
    public void show() {

    }

    @Override
    public void resize(int width, int height) {

    }

    @Override
    public void pause() {

    }

    @Override
    public void resume() {

    }

    @Override
    public void hide() {

    }

    protected void updateGameObjects(float dt) {
        for (int keyIndex = 0; keyIndex < keysInOrder.size(); ++keyIndex) {
            ArrayList<GameObject> goList = gameObjects.get(keysInOrder.get(keyIndex));

            for (int i = 0; i < goList.size(); ++i) {
                goList.get(i).update(dt);
            }
        }
    }

    protected void renderGameObjects(SpriteBatch batch) {
        for (int keyIndex = 0; keyIndex < keysInOrder.size(); ++keyIndex) {
            ArrayList<GameObject> goList = gameObjects.get(keysInOrder.get(keyIndex));
            for (int i = 0; i < goList.size(); ++i) {
                goList.get(i).render(batch);
            }
        }
    }

    protected void renderTextObjects(SpriteBatch batch) {
        for (TextObject object : textObjects) {
            float x = guiCam.position.x - (cam.position.x - object.getX()) * guiCamToCamW;
            float y = guiCam.position.y - (cam.position.y - object.getY()) * guiCamToCamH;
            object.getFont().draw(batch, object.getLayout(), x, y);
        }
    }

    protected abstract void onStart();
    protected abstract void handleInput();
    protected abstract void update(float dt);
    public abstract void dispose();

    private void updateKeysInOrderList() {
        Set<Integer> keySet = gameObjects.keySet();

        for (int key: keySet) {
            if (!keysInOrder.contains(key)) {
                keysInOrder.add(key);
            }
        }
        for (int startIndex = 0; startIndex < keysInOrder.size(); ++startIndex) {
            int smallest = keysInOrder.get(startIndex);
            int smallestIndex = startIndex;
            for (int i = startIndex + 1; i < keysInOrder.size(); ++i) {
                if (smallest > keysInOrder.get(i)) {
                    smallest = keysInOrder.get(i);
                    smallestIndex = i;
                }
            }
            int current = keysInOrder.get(startIndex);
            keysInOrder.set(startIndex, smallest);
            keysInOrder.set(smallestIndex, current);
        }
    }


}
