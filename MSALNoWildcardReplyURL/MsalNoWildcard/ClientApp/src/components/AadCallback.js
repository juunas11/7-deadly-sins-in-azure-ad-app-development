import React from 'react';
import { Redirect } from 'react-router';
import { getUser } from './AuthenticationService';
import { getStoredPath, clearStoredPath } from './LocalRedirectUrlStorage';

const isValidUrl = (url) => {
  const whitelist = [
    /^\/$/,
    /^\/counter\/?$/,
    /^\/fetch-data\/?-?\d*$/
  ];

  return whitelist.some(r => r.test(url));
}

export default class AadCallback extends React.Component {
  constructor(props) {
    super(props);
    this.state = {
      user: null
    };
  }

  componentDidMount() {
    const user = getUser();
    this.setState({ user: user });
  }

  render() {
    if (!this.state.user) {
      return (<div>
        <p>Sorry, something went wrong with login</p>
      </div>)
    }

    let localUrl = getStoredPath();

    if (!isValidUrl(localUrl)) {
      console.warn('Invalid local redirect URL, redirecting to root instead');
      localUrl = '/';
    }

    clearStoredPath();

    return <Redirect to={localUrl} />
  }
}