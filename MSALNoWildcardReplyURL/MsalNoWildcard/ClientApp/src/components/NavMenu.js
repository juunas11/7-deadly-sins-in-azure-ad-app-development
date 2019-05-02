import React from 'react';
import { Collapse, Navbar, NavbarBrand, NavbarToggler, NavItem, NavLink, Nav, UncontrolledDropdown, DropdownToggle, DropdownMenu, DropdownItem } from 'reactstrap';
import { Link } from 'react-router-dom';
import './NavMenu.css';
import { isLoggedIn, getUser, logout } from './AuthenticationService';

export default class NavMenu extends React.Component {
  constructor(props) {
    super(props);

    this.toggle = this.toggle.bind(this);
    this.state = {
      isOpen: false
    };
  }
  toggle() {
    this.setState({
      isOpen: !this.state.isOpen
    });
  }
  render() {
    return (
      <header>
        <Navbar color="dark" dark expand="md">
          <NavbarBrand tag={Link} to="/">MSAL.js no wildcard reply URL</NavbarBrand>
          <NavbarToggler onClick={this.toggle} />
          <Collapse isOpen={this.state.isOpen} navbar>
            <Nav className="ml-auto" navbar>
              <NavItem>
                <NavLink tag={Link} to="/">Home</NavLink>
              </NavItem>
              <NavItem>
                <NavLink tag={Link} to="/counter">Counter</NavLink>
              </NavItem>
              <NavItem>
                <NavLink tag={Link} to="/fetch-data">Fetch data</NavLink>
              </NavItem>
              {!isLoggedIn() && <NavItem>
                <NavLink tag={Link} to="/login">Login</NavLink>
              </NavItem>}
              {isLoggedIn() && <UncontrolledDropdown nav inNavbar>
                <DropdownToggle nav caret>
                  {getUser().displayableId}
                </DropdownToggle>
                <DropdownMenu right>
                  <DropdownItem onClick={() => logout()}>
                    Log out
                  </DropdownItem>
                </DropdownMenu>
              </UncontrolledDropdown>}
            </Nav>
          </Collapse>
        </Navbar>
      </header>
    );
  }
}
