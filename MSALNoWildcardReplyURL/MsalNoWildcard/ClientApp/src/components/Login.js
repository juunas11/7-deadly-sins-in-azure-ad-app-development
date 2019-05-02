import React from 'react';
import { login } from './AuthenticationService';

export default class Login extends React.Component {
  componentDidMount() {
    // trigger login with MSAL
    login();
  }

  render() {
    return <div></div>
  }
}
