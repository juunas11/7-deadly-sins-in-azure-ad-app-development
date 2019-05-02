const SessionStorageKey = 'local-url';

export const storeCurrentPath = () => {
  window.sessionStorage.setItem(SessionStorageKey, window.location.pathname);
};

export const getStoredPath = () => {
  return window.sessionStorage.getItem(SessionStorageKey);
};

export const clearStoredPath = () => {
  window.sessionStorage.removeItem(SessionStorageKey);
};