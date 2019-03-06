var lib = require('lib');
var lang = require('lang!static/noolite/lang.json');

var layoutTemplate = require('static/noolite/web-ui/list.tpl');
var itemTemplate = require('static/noolite/web-ui/list-item.tpl');

//#region views

var ItemView = lib.marionette.View.extend({
	template: lib.handlebars.compile(itemTemplate),
	templateContext: { lang: lang },
	className: 'th-list-item',
	tagName: 'li'
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
        
		this.view.showChildView('list', listView);
	}
});

module.exports = Section;