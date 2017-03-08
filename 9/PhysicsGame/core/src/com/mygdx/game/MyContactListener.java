package com.mygdx.game;

import com.badlogic.gdx.physics.box2d.Contact;
import com.badlogic.gdx.physics.box2d.ContactImpulse;
import com.badlogic.gdx.physics.box2d.ContactListener;
import com.badlogic.gdx.physics.box2d.Manifold;
import com.mygdx.mygameutilities.PhysicsGameObject;

/**
 * Created by atter on 28-Feb-17.
 */

public class MyContactListener implements ContactListener {

    @Override
    public void beginContact(Contact contact) {
        PhysicsGameObject go1 = (PhysicsGameObject)contact.getFixtureA().getBody().getUserData();
        PhysicsGameObject go2 = (PhysicsGameObject)contact.getFixtureB().getBody().getUserData();
        if (go1 != null && go2 != null) {
            go1.onCollision(go2);
            go2.onCollision(go1);
        }

    }

    @Override
    public void endContact(Contact contact) {

    }

    @Override
    public void preSolve(Contact contact, Manifold oldManifold) {

    }

    @Override
    public void postSolve(Contact contact, ContactImpulse impulse) {
    }

}