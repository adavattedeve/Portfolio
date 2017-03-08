package com.mygdx.mygameutilities;

import com.badlogic.gdx.graphics.g2d.SpriteBatch;

import java.util.Stack;

/**
 * Created by atter on 28-Jan-17.
 */

public class GameStateManager {

    private Stack<State> states;
    private MyGame game;

    public GameStateManager(MyGame game) {
        this.game = game;
        states = new Stack<State>();
    }

    public void push(State state) {
        states.push(state);
        game.setScreen(state);
        state.onStart();
    }

    public void set(State state) {
        if (!states.isEmpty()) {
            states.peek().disposeState();
            states.pop();
        }
        states.push(state);
        game.setScreen(state);
        state.onStart();
    }

    public void pop() {
        if (!states.isEmpty()) {
            states.peek().dispose();
            states.pop();
        }
        if (!states.isEmpty()) {
            game.setScreen(states.peek());
        }
    }

    public State peek() {
        return states.peek();
    }

    public void update(float dt) {
        if (!states.isEmpty())
            states.peek().updateState(dt);
    }

    public void render(SpriteBatch sb) {
        if (!states.isEmpty()) {
            states.peek().render(sb);
        }

    }

    public void dispose() {
        while(!states.isEmpty()) {
            states.peek().disposeState();
            states.pop();
        }
    }
}
