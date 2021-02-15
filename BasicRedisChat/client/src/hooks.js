// @ts-check
import { useEffect, useState } from "react";
import { getMe, login, logOut } from "./api";


/** User management hook. */
const useUser = (onUserLoaded = (user) => { }, dispatch) => {
  const [loading, setLoading] = useState(true);
  /** @type {[import('./state.js').UserEntry | null, React.Dispatch<import('./state.js').UserEntry>]} */
  const [user, setUser] = useState(null);
  /** Callback used in log in form. */
  const onLogIn = (
    username = "",
    password = "",
    onError = (val = null) => { },
    onLoading = (loading = false) => { }
  ) => {
    onError(null);
    onLoading(true);
    login(username, password)
      .then((x) => {
        setUser(x);
        onLoading(false);
      })
      .catch((e) => {
        onError(e.message);
        onLoading(false);
      });
  };

  /** Log out form */
  const onLogOut = async () => {
    logOut().then(() => {
      setUser(null);
      /** This will clear the store, to completely re-initialize an app on the next login. */
      dispatch({ type: "clear" });
      setLoading(true);
    });
  };

  /** Runs once when the component is mounted to check if there's user stored in cookies */
  useEffect(() => {
    if (!loading) {
      return;
    }
    getMe().then((user) => {
      setUser(user);
      setLoading(false);
      onUserLoaded(user);
    });
  }, [onUserLoaded, loading]);

  return { user: typeof user === "string" ? null : user, onLogIn, onLogOut, loading };
};

export {
  useUser
};