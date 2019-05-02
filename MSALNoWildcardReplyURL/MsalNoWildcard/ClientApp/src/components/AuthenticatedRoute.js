import React from 'react';
import { Route, Redirect } from 'react-router';
import { isLoggedIn } from './AuthenticationService';
import { storeCurrentPath } from './LocalRedirectUrlStorage';

const renderRedirect = () => {
  storeCurrentPath();

  return <Redirect to='/login' />;
};

const AuthenticatedRoute = ({ component: Component, ...rest }) => (
  <Route {...rest} render={(props) => (
    isLoggedIn()
      ? <Component {...props} />
      : renderRedirect()
  )} />
)

export default AuthenticatedRoute;