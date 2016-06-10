$(function() {
    $.widget("ui.groupdocsSignature", {
        options: {
            previewDocument: '',
            hostUrl: '',
            showHeader: true,
            documentGuid: '',
            recipientGuid: '',
            showTypeItTab: true,
            showUploadTab: true,
            signaturePenColors: []
        },
        signDoc: null,

        on: function(eventName, handler) {
            $(this.element).on(eventName, handler);
        },

        off: function(eventName, handler) {
            $(this.element).off(eventName, handler);
        },

        _addHtml: function () {
            $(this.element).empty();
            $(this.element).html(
        "<div class='loading_overlay embed' style='display: none; top: 60px;'><div class='loading_overlay_message' style=' width: 240px;'><span class='progresspin'></span><p>Signing your document...</p></div></div>\
            <div class='viewer_header header_sidescroll'>\
                <div class='viewer_header_wrapper'>\
                    <a class='btnOpen new_head_tools_btn h_t_i_browser' data-tooltip='Open File' data-localize-tooltip='OpenFile'></a> \
                    <div name='printAndDownloadToolbar' class='new_head_tools_wrapper left'>\
                        <a class='new_head_tools_btn h_t_i_download btn_download' data-tooltip='Download' data-localize-tooltip='Download'></a>\
                        <a class='new_head_tools_btn h_t_i_print print_button' data-tooltip='Print' data-localize-tooltip='Print'></a>\
                    </div>\
                    <!-- ko stopBindings: true -->\
                    <div id='viewer-navigation' data-bind=\"template: 'widgets/document-navigation'\">\
                    </div>\
                    <!-- /ko -->\
                    <div id='viewer-zoom' class='' style='display:inline-block'>\
                    </div>\
                </div>\
            </div>\
            <div class='fileOpenDialogWrapper' style='display: none'></div>\
            <!-- Main content -->\
            <div id='viewer_mainwrapper' class='embedded_viewer_wrapper embed_signature'>\
                <!-- ko template: 'widgets/document-thumbnails' --><!-- /ko -->\
                <!-- ko stopBindings: true -->\
                <div id='doc_viewer_wrapper' class='document_viewer' style='overflow: auto; bottom: 40px;' data-bind=\"template: 'widgets/sign-document-viewer', event: { scroll: function (item, e) { this.ScrollDocView(item, e); return true; }, scrollstop: function (item, e) { this.ScrollDocViewEnd(item, e); return true; } }\">\
                </div>\
                <!-- /ko -->\
                <!-- ko stopBindings: true -->\
                <div class='embed_signature_footer sign_button_container' style='position: absolute;'>\
                    <button class='red_button sign_document' rel='tooltip' data-bind=\"tooltip: {}, enableSign: { fieldsLeft: fieldsToBeFilled(), recipientStatus: 1, app: 'document' }\" data-placement='top' data-html='true'>Confirm Signature</button>\
                </div>\
                <!-- /ko -->\
            </div>\
            <!-- ko stopBindings: true -->\
            <div id='signature-dialog' class='signature-dialog' data-bind=\"template: 'widgets/create-signature-dialog'\"></div>\
            <!-- /ko -->\
            <!-- ko stopBindings: true -->\
            <div id='errorDialog' class='modal fade modal2 modal800px' data-bind='modal: isVisible,  modalOptions: { beforeShow: onBeforeShow, beforeClose: onBeforeClose }' data-keyboard='false' data-backdrop='false'>\
                <div class='modal_inner_wrapper'>\
	                <div class='modal_header'>\
                        <h3>Error</h3>\
                    </div>\
                    <div class='modal_content'>\
		                <div class='modal_input_wrap_left'>\
                            <div data-bind=\"text: errorText\"></div>\
                        </div>\
                    </div>\
                    <div class='modal_footer'>\
                        <div class='modal_btn_wrapper'>\
                        </div>\
                    </div>\
                </div>\
            </div>\
            <!-- /ko -->\
            <!-- ko stopBindings: true -->\
            <div id='enter-name-dialog' data-bind=\"template: 'widgets/enter-name-dialog'\"></div>\
            <!-- /ko -->\
            <div id='jerror' title='Error' class='modal fade modal2'>\
                <div class='modal_inner_wrapper'>\
                    <a class='popclose' data-dismiss='modal'></a>\
                    <div class='modal_header'>\
                        <h3>Error</h3>\
                    </div>\
                    <div class='modal_content'>\
                        <div class='modal_input_wrap_left'>\
                            <div id='jerrorMsg'></div>\
                        </div>\
                    </div>\
                    <div class='modal_footer'>\
                        <div class='modal_btn_wrapper'>\
                            <a href='#' data-dismiss='modal' class='grey_button right'>Close</a>\
                        </div>\
                    </div>\
                </div>\
            </div>");

        },

        // the constructor
        _create: function (options) {
            var self = this;
            require(['signDocument'], function (signDocument) {
                self._addHtml();
                self.signDoc = new signDocument();
                self.signDoc.init($(self.element), self.options);
            });
            //self.on("onDocumentLoadComplete", function(event, data) {
            //    if (!data.lic) {
            //        var viewerMainWrapper = $(self.element).find(".embedded_viewer_wrapper");
            //        $(viewerMainWrapper).css("top", "94px");
            //        $(self.element).find(".banner_trial").remove();
            //        var licElement = $("<div/>");
            //        $(licElement).addClass("banner_trial");
            //        $(licElement).html("This viewer has been created using an unlicensed version of " +
            //            "<a href='http://groupdocs.com' target='_blank'>GroupDocs.Signature</a> for .NET ");
            //        $(licElement).insertBefore(viewerMainWrapper);
            //    };
            //});

            var groupdocsViewerWrapper;
            var container = this.element;
            var settings = $.extend(
                { hostUrl: this.applicationPath }, options);
            this.printImageElements = new Array();

            //this.groupdocsViewerWrapper = groupdocsViewerWrapper = container.find(".groupdocs_viewer_wrapper");
            this.groupdocsViewerWrapper = groupdocsViewerWrapper = container;
            var viewerHeader = this.viewerHeader = groupdocsViewerWrapper.find(".viewer_header");
            var viewerMainWrapper = this.viewerMainWrapper = groupdocsViewerWrapper.find(".embedded_viewer_wrapper");
            var fileOpenDialogWrapper = this.fileOpenDialogWrapper = groupdocsViewerWrapper.find(".fileOpenDialogWrapper");

            //if (settings.showFolderBrowser) {
            var fileOpenDialogOptions = {
                hostUrl: settings.hostUrl,
                userId: settings.userId,
                userKey: settings.userKey,
                fileExplorer: groupdocsViewerWrapper.find(".file_browser_content"),
                fileUploader: groupdocsViewerWrapper.find(".file_browser_toolbar"),
                resourcePrefix: settings.resourcePrefix,
                urlHashEnabled: false,
                instanceIdToken: settings.instanceIdToken
            };
            fileOpenDialogWrapper.fileOpenDialog(fileOpenDialogOptions);
            fileOpenDialogWrapper.find(".popclose").click(function () {
                self._hideFileOpenDialog();
            });
            //}
        }
    });
});