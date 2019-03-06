var lib = require('lib');
var lang = require('lang!static/noolite/lang.json');

var layoutTemplate = require('static/noolite/web-ui/list.tpl');
var itemTemplate = require('static/noolite/web-ui/list-item.tpl');

//#region views

var ItemView = lib.marionette.View.extend({
	template: lib.handlebars.compile(itemTemplate),
	templateContext: { lang: lang },
	className: 'th-list-item',
    tagName: 'li',
	triggers: {
        'click .js-channel-on': 'on',
        'click .js-channel-off': 'off',
        'click .js-channel-bind': 'bind',
        'click .js-channel-unbind': 'unbind'
	}
});

var ListView = lib.marionette.CollectionView.extend({
	childView: ItemView,
	className: 'list-unstyled',
    tagName: 'ul',
	childViewTriggers: {
        'on': 'noolite:on',
        'off': 'noolite:off',
        'bind': 'noolite:bind',
        'unbind': 'noolite:unbind'
	}
});

var LayoutView = lib.marionette.View.extend({
	template: lib.handlebars.compile(layoutTemplate),
	templateContext: { lang: lang },
	regions: {
		list: '.js-task-list'
	}
});

//#endregion

var Section = lib.common.AppSection.extend({
	start: function () {
		this.view = new LayoutView();
		this.listenTo(this.view, 'task:create', this.bind('createTask'));

		this.application.setContentView(this.view);

		return lib.ajax
			.loadModel('/api/noolite/web-api/list', lib.backbone.Collection)
			.then(this.bind('displayList'));
	},

	displayList: function (items) {
        var listView = new ListView({ collection: items });

        this.listenTo(listView, 'noolite:on', this.bind('onNooliteOn'));
        this.listenTo(listView, 'noolite:off', this.bind('onNooliteOff'));
        this.listenTo(listView, 'noolite:bind', this.bind('onNooliteBind'));
        this.listenTo(listView, 'noolite:unbind', this.bind('onNooliteUnbind'));
        
		this.view.showChildView('list', listView);
	},
	onNooliteOn: function (childView) {
        var channel = childView.model.get('Channel');

        var result = lib.ajax.loadModel(`/api/noolite/web-api/channel?ch=${channel}&command=on`);

        console.log(result);
	},
	onNooliteOff: function (childView) {
		var channel = childView.model.get('Channel');

        var result = lib.ajax.loadModel(`/api/noolite/web-api/channel?ch=${channel}&command=off`);

		console.log(channel);
	},
	onNooliteBind: function (childView) {
		var channel = childView.model.get('Channel');

		var result = lib.ajax.loadModel(`/api/noolite/web-api/channel?ch=${channel}&command=bind`);

		console.log(channel);
	},
	onNooliteUnbind: function (childView) {
		var channel = childView.model.get('Channel');

		var result = lib.ajax.loadModel(`/api/noolite/web-api/channel?ch=${channel}&command=unbind`);

		console.log(channel);
	}
});

module.exports = Section;