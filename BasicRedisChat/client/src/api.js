import axios from 'axios';
axios.defaults.withCredentials = true;

const BASE_URL = '';

export const MESSAGES_TO_LOAD = 15;

const url = x => `${BASE_URL}${x}`;

/** Checks if there's an existing session. */
export const getMe = () => {
  return axios.get(url('/users/me'))
    .then(x => x.data)
    .catch(_ => null);
};

/** 
 * Fetch users by requested ids
 * @param {Array<number | string>} ids
 */
export const getUsers = (ids) => {
  return axios.get(url(`/users`), { params: { ids : ids || [] } }).then(x => x.data);
};

/** Fetch users which are online */
export const getOnlineUsers = () => {
  return axios.get(url(`/users/online`)).then(x => x.data);
};

/** Handle user log in */
export const login = (username, password) => {
  return axios.post(url('/auth/login'), {
    username,
    password
  }).then(x =>
    x.data
  )
    .catch(e => { throw new Error(e.response && e.response.data && e.response.data.message); });
};

export const logOut = () => {
  return axios.post(url('/auth/logout'));
};

/** 
 * Function for checking which deployment urls exist.
 * 
 * @returns {Promise<{
 *   heroku?: string;
 *   google_cloud?: string;
 *   vercel?: string;
 *   github?: string;
 * }>} 
 */
export const getButtonLinks = () => {
  return axios.get(url('/links'))
    .then(x => x.data)
    .catch(_ => null);
};

/** 
 * @returns {Promise<Array<{ names: string[]; id: string }>>} 
 */
export const getRooms = async (userId) => {
  return axios.get(url(`/rooms/user/${userId}`)).then(x => x.data);
};

/**
 * Load messages
 * 
 * @param {string} id room id
 * @param {number} offset 
 * @param {number} size 
 */
export const getMessages = (id,
  offset = 0,
  size = MESSAGES_TO_LOAD
) => {
  return axios.get(url(`/rooms/messages/${id}`), {
    params: {
      offset,
      size
    }
  })
    .then(x => x.data.reverse());
};

/** This one is called on a private messages room created. */
export const addRoom = async (user1, user2) => {
  return axios.post(url(`/room`), { user1, user2 }).then(x => x.data);
};
