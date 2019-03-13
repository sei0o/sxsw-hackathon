/*
  clang++ -Wall -std=c++14 -I pointer to /nlohman/json/include -emit-llvm -fno-use-cxa-atexit -c -g helloWorld.cpp -o helloWorld.bc
*/

#include <stdio.h>
#include <string.h>
#include <stdlib.h>
#include <unistd.h>
#include <sys/stat.h>
#include <string>
#include <unordered_map>
#include <vector>
#include "nlohmann/json.hpp"
#include "eluvio/utils.h"
#include "eluvio/el_cgo_interface.h"
#include "eluvio/bitcode_context.h"
#include "eluvio/media.h"
using namespace elv_context;

using nlohmann::json;

/*
 * Outputs the data from key "image"
 */
elv_return_type make_image(BitCodeCallContext* ctx)
{
    return elv_media_fns::make_image(ctx);
}

elv_return_type content(BitCodeCallContext* ctx,  JPCParams& p)
{
    auto path = ctx->HttpParam(p, "path");
    if (path.second.IsError()){
        return ctx->make_error("getting path from JSON", path.second);
    }

    char* pContentRequest = (char*)(path.first.c_str());

    LOG_INFO(ctx, "content", pContentRequest);

    if (strcmp(pContentRequest, "/image") == 0) {
        LOG_INFO(ctx, "making REP /image");
        return make_image(ctx);
    }

    const char* msg = "unknown type requested";
    return ctx->make_error(msg, E(msg).Kind(E::Other));

}

std::pair<std::shared_ptr<std::vector<uint8_t>>, uint32_t> readPartForHash(BitCodeCallContext *ctx, std::string &phash) {
    uint32_t psz = 0;
    auto body = ctx->QReadPart(phash.c_str(), 0, -1, &psz);
    if (body->size() == 0) {
        const char* msg = "Failed to read resource part";
        LOG_ERROR(ctx, msg, "HASH", phash.c_str());
        return make_pair(nullptr, 0);
    }
    LOG_INFO(ctx, "generate",  "thumbnail_part_size", (int)psz);
    return make_pair(body, psz);
}

elv_return_type generate(BitCodeCallContext* ctx,  JPCParams& p)
{
    auto qparams = ctx->QueryParams(p);
    if (qparams.second.IsError()) {
        return ctx->make_error("could not read query parameters", qparams.second);
    }

    if (qparams.first.find("other_qhash") == qparams.first.end()) {
        return ctx->make_error("could not find other_qhash parameter", E::BadHttpParams);
    }

    LOG_DEBUG(ctx, "generate: image 1");
    auto phash = ctx->SQMDGetString("image");
    if (phash == "") {
        return ctx->make_error("generate", E("generate", "image", "image"));
    }
    LOG_INFO(ctx, "generate",  "qphash", phash.c_str());
    // return ctx->make_error(phash.c_str(), E("generate", "image", phash.c_str()));
    
    // load the first image
    auto part = readPartForHash(ctx, phash);
    if (!part.first) return ctx->make_error(phash.c_str(), E("generate", "imge", phash.c_str()));
    
    // load the other image
    auto otherPart = readPartForHash(ctx, qparams.first["other_qhash"]);

    ctx->Callback(200, "image/png", otherPart.second);
    // ctx->WriteOutput(*otherPart.first);

    // generate a new image
    std::vector<uint8_t> result(otherPart.second);
    for (unsigned long i = 0; i < otherPart.first->size(); ++i) {
        result.push_back((*part.first)[i] ^ (*otherPart.first)[i]);
    }

    ctx->WriteOutput(result);

    return ctx->make_success();
}

BEGIN_MODULE_MAP()
    MODULE_MAP_ENTRY(content)
    MODULE_MAP_ENTRY(generate)
END_MODULE_MAP()
