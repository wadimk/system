var lib = require('lib');
var lang = require('lang!static/scripts/web-ui/lang.json');

var layoutTemplate = require('static/scripts/web-ui/list.tpl');
var itemTemplate = require('static/scripts/web-ui/list-item.tpl');

//#region entities

var ScriptModel = lib.backbone.Model.extend({});

var ScriptCollection = lib.backbone.Collection.extend({
    model: ScriptModel
});

//#endregion


//#region views

var ItemView = lib.marionette.View.extend({
    template: lib.handlebars.compile(itemTemplate),
    tagName: 'li',
    className: 'th-list-item',
    triggers: {
        'click .js-script-edit': 'scripts:edit',
        'click .js-script-execute': 'scripts:execute'
    }
});

var ListView = lib.marionette.CollectionView.extend({
    childView: ItemView,
    className: 'list-unstyled',
    tagName: 'ul'
});

var LayoutView = lib.marionette.View.extend({
    template: lib.handlebars.compile(layoutTemplate),
    templateContext: { lang: lang },
    regions: {
        list: '.js-script-list'
    },
    triggers: {
        'click .js-script-add': 'scripts:create'
    }
});

//#endregion

var Section = lib.common.AppSection.extend({
    start: function () {
        this.view = new LayoutView();
        this.listenTo(this.view, 'scripts:create', this.bind('addScript'));

        this.application.setContentView(this.view);

        return lib.ajax
            .loadModel('/api/scripts/web-api/list', ScriptCollection)
            .then(this.bind('displayList'));
    },

    displayList: function (items) {
        var listView = new ListView({ collection: items });

        this.listenTo(listView, 'childview:scripts:edit', this.bind('editScript'));
        this.listenTo(listView, 'childview:scripts:execute', this.bind('executeScript'));
        this.view.showChildView('list', listView);
    },

    addScript: function () {
        this.application.navigate('/static/scripts/web-ui/editor.js');
    },

    editScript: function (view) {
        var scriptId = view.model.get('id');
        this.application.navigate('/static/scripts/web-ui/editor.js', scriptId);
    },

    executeScript: function (view) {
        var scriptId = view.model.get('id');
        lib.ajax.getJSON('/api/scripts/web-api/execute', { id: scriptId })
            .then(
                function () { alert(lang.get('The script has been executed.')); },
                function (err) { alert(err.message); });
    }
});

module.exports = Section;