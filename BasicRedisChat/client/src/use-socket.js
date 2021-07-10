// @ts-check
import { useEffect, useRef, useState } from "react";
// eslint-disable-next-line no-unused-vars
import io, { Socket } from "socket.io-client";
import { parseRoomName } from "./utils";
import * as signalR from '@microsoft/signalr';
/**
 * @param {import('./state').UserEntry} newUser
 */
const updateUser = (newUser, dispatch, infoMessage) => {
  dispatch({ type: "set user", payload: newUser });
  if (infoMessage !== undefined) {
    dispatch({
      type: "append message",
      payload: {
        id: "0",
        message: {
          /** Date isn't shown in the info message, so we only need a unique value */
          date: Math.random() * 10000,
          from: "info",
          message: infoMessage,
        },
      },
    });
  }
};

/** @returns {[Socket, boolean, () => void]} */
const useSocket = (user, dispatch, onLogOut) => {
  const [connected, setConnected] = useState(false);
  /** @type {React.MutableRefObject<signalR.HubConnection>} */
  const socketRef = useRef(null);
  const socket = socketRef.current;

  /** First of all it's necessary to handle the socket io connection */
  useEffect(() => {
    if (user === null) {
      if (socket !== null) {
        socketRef.current.stop().then(() => {
          socketRef.current = null;
          setConnected(false);
        });
      }
    } else {
      if (socketRef.current === null) {
        socketRef.current = new signalR.HubConnectionBuilder()
          .withUrl("/chat")
          .build();
        socketRef.current.start().then(() => {
          debugger
          //socketRef.current.invoke("OnLogIn", JSON.stringify(user));
          setConnected(true);
        });
      }
    }
  }, [user, socket]);

  /**
   * Once we are sure the socket io object is initialized
   * Add event listeners.
   */
  useEffect(() => {
    if (connected && user) {
      socket.on("user.connected", (username) => {
        updateUser(username, dispatch, `${username} connected`);
      });
      socket.on("user.disconnected", (username) => {
        updateUser(username, dispatch, `${username} left`);
      });
      socket.on("message", (message) => {
        /** Set user online */
        message = JSON.parse(message);

        dispatch({
          type: "make user online",
          payload: message.from,
        });
        dispatch({
          type: "append message",
          payload: { id: message.roomId === undefined ? "0" : message.roomId, message },
        });
      });
    } else {
      /** If there was a log out, we need to clear existing listeners on an active socket connection */
      if (socket) {
        socket.off("user.connected");
        socket.off("user.disconnected");
        socket.off("user.room");
        socket.off("message");
      }
    }
  }, [connected, user, dispatch, socket]);

  return [{
    // @ts-ignore
    emit(_, message) {
      socket.invoke("SendMessage", JSON.stringify(user), JSON.stringify(message));
      return {};
    }
  }, true,
  () => {
    //socket.invoke("OnLogOut", JSON.stringify(user));
    onLogOut();
  }];
};

export { useSocket };
