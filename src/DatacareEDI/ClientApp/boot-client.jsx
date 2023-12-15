"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
require("./css/site.css");
require("bootstrap");
const React = require("react");
const ReactDOM = require("react-dom");
const react_router_1 = require("react-router");
const react_redux_1 = require("react-redux");
const react_router_redux_1 = require("react-router-redux");
const routes_1 = require("./routes");
const configureStore_1 = require("./configureStore");
const initialState = window.initialReduxState;
const store = configureStore_1.default(initialState);
const history = react_router_redux_1.syncHistoryWithStore(react_router_1.browserHistory, store);
ReactDOM.render(<react_redux_1.Provider store={store}>
        <react_router_1.Router history={history} children={routes_1.default}/>
    </react_redux_1.Provider>, document.getElementById('react-app'));
//# sourceMappingURL=boot-client.jsx.map